namespace ShowcaseClient.Pages.Dashboard
{
    public partial class SyncStatus
    {
        Exception? syncException;

        protected override void OnInitialized()
        {
            _dataSynchronizer.SynchronizeInBackground();
            _dataSynchronizer.OnUpdate += StateHasChanged;
            _dataSynchronizer.OnError += HandleSyncError;
        }

        public void Dispose()
        {
            _dataSynchronizer.OnUpdate -= StateHasChanged;
            _dataSynchronizer.OnError -= HandleSyncError;
        }

        void HandleSyncError(Exception ex)
        {
            syncException = ex;
            StateHasChanged();
        }
    }
}
