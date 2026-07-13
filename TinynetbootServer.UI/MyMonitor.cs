using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TinynetbootServer.Core
{
    public class MyMonitor
    {
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private System.Windows.Forms.Timer _timer;

        public event Action<float>? OnCpuUpdated;
        public event Action<float>? OnRamUpdated;

        public MyMonitor(int intervalMs = 1000)
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                // "Available MBytes" tells us how much RAM is free
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

                _timer = new System.Windows.Forms.Timer();
                _timer.Interval = intervalMs;
                _timer.Tick += (s, e) =>
                {
                    OnCpuUpdated?.Invoke(_cpuCounter.NextValue());
                    OnRamUpdated?.Invoke(_ramCounter.NextValue());
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Monitor Error: " + ex.Message);
            }
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public bool IsRunning => _timer.Enabled;
    }
}