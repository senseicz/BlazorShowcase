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

    private readonly int _totalScoresToDownload = 500;
    private int _scoresDownloaded = 0;

    private readonly ILogger<DataSynchronizer> _logger;


    public DataSynchronizer(IJSRuntime js, 
        IDbContextFactory<ClientSideDbContext> dbContextFactory, 
        ScoresData.ScoresDataClient scoresData,
        ILogger<DataSynchronizer> logger)
    {
        _logger = logger;

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
        _logger.LogInformation("[{now}] FirstTimeSetupTask start.", DateTime.Now);
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./dbstorage.js");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser")))
        {
            await module.InvokeVoidAsync("synchronizeFileWithIndexedDb", SqliteDbFilename);
        }

        using var db = await dbContextFactory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();

        _logger.LogInformation("[{now}] FirstTimeSetupTask finish.", DateTime.Now);
    }

    private async Task EnsureSynchronizingAsync()
    {
        // Don't run multiple syncs in parallel. This simple logic is adequate because of single-threadedness.
        if (isSynchronizing)
        {
            _logger.LogInformation("[{now}] IsSyncing: TRUE", DateTime.Now);
            return;
        }

        try
        {
            _logger.LogInformation("[{now}] Synchronization START", DateTime.Now);


            isSynchronizing = true;
            SyncCompleted = 0;
            SyncTotal = _totalScoresToDownload;

            // Get a DB context

            _logger.LogInformation("[{now}] Before GetPreparedDbContextAsync()", DateTime.Now);

            using var db = await GetPreparedDbContextAsync();
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _logger.LogInformation("[{now}] After GetPreparedDbContextAsync()", DateTime.Now);

            // Begin fetching any updates to the dataset from the backend server
            //var mostRecentUpdate = db.Scores.OrderByDescending(p => p.CreatedOn).FirstOrDefault()?.CreatedOn;

            _logger.LogInformation("[{now}] Before GetDbConnection()", DateTime.Now);

            var connection = db.Database.GetDbConnection();
            connection.Open();

            _logger.LogInformation("[{now}] DB Connection is open now.", DateTime.Now);

            _logger.LogInformation("[{now}] Sending FIRST GetScores request.", DateTime.Now);

            var request = new ScoreRequest {TotalRequested = _totalScoresToDownload, Downloaded = _scoresDownloaded};
            var response = await scoresData.GetScoresAsync(request);

            _logger.LogInformation("[{now}] Response received, start with bulk insert.", DateTime.Now);

            BulkInsert(connection, response.Scores);
            OnUpdate?.Invoke();

            _scoresDownloaded = response.Count;

            _logger.LogInformation("[{now}] Bulk update finish.", DateTime.Now);

            var counter = 1;

            while (true)
            {
                counter++;

                //_logger.LogInformation("[{now}] Sending GetScores request number {reqNumber}", DateTime.Now, counter);

                request = new ScoreRequest { TotalRequested = _totalScoresToDownload, Downloaded = _scoresDownloaded };
                response = await scoresData.GetScoresAsync(request);

                //_logger.LogInformation("[{now}] Response of request no: {reqNo} received, start with bulk insert.", DateTime.Now, counter);


                _scoresDownloaded += response.Count;
                SyncCompleted = _scoresDownloaded;

                if (response.Count == 0)
                {
                    break;
                }
                else
                {
                    //_logger.LogInformation("[{now}] Bulk insert of of request no: {reqNo} START.", DateTime.Now, counter);
                    //mostRecentUpdate = response.Parts.Last().ModifiedTicks;
                    BulkInsert(connection, response.Scores);
                    OnUpdate?.Invoke();

                    //_logger.LogInformation("[{now}] Bulk insert of of request no: {reqNo} STOP.", DateTime.Now, counter);
                }
            }
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

        _logger.LogInformation("[{now}] BEFORE OnUpdate.Invoke().", DateTime.Now);
        OnUpdate?.Invoke();
        _logger.LogInformation("[{now}] AFTER OnUpdate.Invoke().", DateTime.Now);

    }

    private void BulkInsert(DbConnection connection, IEnumerable<Score> scores)
    {
        //_logger.LogInformation("[{now}] Inserting {count} Scores - START",  DateTime.Now, scores.Count());


        // Since we're inserting so much data, we can save a huge amount of time by dropping down below EF Core and
        // using the fastest bulk insertion technique for Sqlite.
        using (var transaction = connection.BeginTransaction())
        {
            var command = connection.CreateCommand();
            var id = AddNamedParameter(command, "$Id");
            var streamId = AddNamedParameter(command, "$StreamId");
            var createdOn = AddNamedParameter(command, "$CreatedOn");
            var userName = AddNamedParameter(command, "$UserName");
            var fullName = AddNamedParameter(command, "$FullName");
            var ipAddress = AddNamedParameter(command, "$IpAddress");
            var city = AddNamedParameter(command, "$City");
            var riskScore = AddNamedParameter(command, "$RiskScore");

            command.CommandText =
                $"INSERT OR REPLACE INTO Scores (Id, StreamId, CreatedOn, UserName, FullName, IpAddress, City, RiskScore) " +
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

            //_logger.LogInformation("[{now}] Scores inserted - STOP", DateTime.Now);
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
