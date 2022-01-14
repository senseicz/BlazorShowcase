using System.Timers;

namespace ShowcaseClient.Data
{
    public class JobExecutedEventArgs : EventArgs { }


    public class PeriodicExecutor : IDisposable
    {
        public event EventHandler<JobExecutedEventArgs> JobExecuted;
        void OnJobExecuted()
        {
            JobExecuted?.Invoke(this, new JobExecutedEventArgs());
        }

        System.Timers.Timer _Timer;
        bool _Running;

        public void StartExecuting()
        {
            if (!_Running)
            {
                // Initiate a Timer
                _Timer = new System.Timers.Timer();
                _Timer.Interval = 1000;  // every sec
                _Timer.Elapsed += HandleTimer;
                _Timer.AutoReset = true;
                _Timer.Enabled = true;

                _Running = true;
            }
        }
        void HandleTimer(object source, ElapsedEventArgs e)
        {
            // Execute required job

            // Notify any subscribers to the event
            OnJobExecuted();
        }
        public void Dispose()
        {
            if (_Running)
            {
                // Clear up the timer
            }
        }
    }
}
