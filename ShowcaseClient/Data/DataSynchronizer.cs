using BlazorShowcase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace ShowcaseClient.Data;

// This service synchronizes the Sqlite DB with both the backend server and the browser's IndexedDb storage
class DataSynchronizer
{
    public const string SqliteDbFilename = "app.db";
    private readonly Task firstTimeSetupTask;
    private readonly IDbContextFactory<ClientSideDbContext> dbContextFactory;
    private readonly ScoresData.ScoresDataClient scoresData;
    private bool isSynchronizing;

    public DataSynchronizer(IJSRuntime js, IDbContextFactory<ClientSideDbContext> dbContextFactory, ScoresData.ScoresDataClient scoresData)
    {
        this.dbContextFactory = dbContextFactory;
        this.scoresData = scoresData;
        firstTimeSetupTask = FirstTimeSetupAsync(js);
    }

    public int SyncCompleted { get; private set; }
    public int SyncTotal { get; private set; }

    public async Task<ClientSideDbContext> GetPreparedDbContextAsync()
    {
        await firstTimeSetupTask;
        return await dbContextFactory.CreateDbContextAsync();
    }

    public void SynchronizeInBackground()
    {
        _ = EnsureSynchronizingAsync();
    }

    public event Action? OnUpdate;
    public event Action<Exception>? OnError;

    private async Task FirstTimeSetupAsync(IJSRuntime js)
    {
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
        }

        using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (isSynchronizing)
        {
            return;
        }

        try
        {
            isSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = 0;

            // Get a DB context
            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // Begin fetching any updates to the dataset from the backend server
            var mostRecentUpdate = db.Scores.OrderByDescending(p => p.CreatedOn).FirstOrDefault()?.CreatedOn;

            var connection = db.Database.GetDbConnection();
            connection.Open();


            var request = new ScoreRequest { /*MaxCount = 5000, ModifiedSinceTicks = mostRecentUpdate ?? -1 */};
            var response = await scoresData.GetScoresAsync(request);

            BulkInsert(connection, response.Scores);
            OnUpdate?.Invoke();


            //while (true)
            //{
            //    var request = new ScoreRequest { /*MaxCount = 5000, ModifiedSinceTicks = mostRecentUpdate ?? -1 */};
            //    var response = await scoresData.GetScoresAsync(request);
            //    var syncRemaining = response.ModifiedCount - response.Parts.Count;
            //    SyncCompleted += response.Parts.Count;
            //    SyncTotal = SyncCompleted + syncRemaining;

            //    if (response.Parts.Count == 0)
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        mostRecentUpdate = response.Parts.Last().ModifiedTicks;
            //        BulkInsert(connection, response.Parts);
            //        OnUpdate?.Invoke();
            //    }
            //}
        }
        catch (Exception ex)
        {
            // TODO: use logger also
            OnError?.Invoke(ex);
        }
        finally
        {
            isSynchronizing = false;
        }
    }

    private void BulkInsert(DbConnection connection, IEnumerable<Score> scores)
    {
        // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
        // using the fastest bulk insertion technique for Sqlite.
        using (var transaction = connection.BeginTransaction())
        {
            var command = connection.CreateCommand();
            var id = AddNamedParameter(command, "Id");
            var streamId = AddNamedParameter(command, "$StreamId");
            var createdOn = AddNamedParameter(command, "$CreatedOn");
            var userName = AddNamedParameter(command, "$UserName");
            var fullName = AddNamedParameter(command, "$FullName");
            var ipAddress = AddNamedParameter(command, "$IpAddress");
            var city = AddNamedParameter(command, "$City");
            var riskScore = AddNamedParameter(command, "$RiskScore");

            command.CommandText =
                $"INSERT OR REPLACE INTO Parts (Id, StreamId, CreatedOn, UserName, FullName, IpAddress, City, RiskScore) " +
                $"VALUES ({id.ParameterName}, {streamId.ParameterName}, {createdOn.ParameterName}, {userName.ParameterName}, {fullName.ParameterName}, {ipAddress.ParameterName}, {city.ParameterName}, {riskScore.ParameterName})";

            foreach (var score in scores)
            {
                id.Value = score.Id;
                streamId.Value = score.StreamId;
                createdOn.Value = score.CreatedOn;
                userName.Value = score.UserName;
                fullName.Value = score.FullName;
                ipAddress.Value = score.IpAddress;
                city.Value = score.City;
                riskScore.Value = score.RiskScore;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        static DbParameter AddNamedParameter(DbCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
