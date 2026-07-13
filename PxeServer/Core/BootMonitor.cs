using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TinynetbootServer.Core
{
    public class BootSession
    {
        public string Ip { get; set; } = "";
        public string Mac { get; set; } = "";
        public string Status { get; set; } = "BOOTING";
    }

    public class BootMonitor
    {
        // Internal list to track machines
        private readonly List<BootSession> _sessions = new();

        // This event tells the UI: "Something changed, please refresh!"
        public event Action<List<BootSession>>? OnMonitorUpdated;

        public void UpdateProgress(string ip, string mac, string status)
        {
            lock (_sessions) // Lock prevents data corruption if 2 PCs boot at the same time
            {
                var session = _sessions.FirstOrDefault(s => s.Ip == ip);
                if (session == null)
                {
                    _sessions.Insert(0, new BootSession { Ip = ip, Mac = mac, Status = status });
                }
                else
                {
                    session.Status = status;
                    if (mac != "Unknown") session.Mac = mac;
                }

                // Tell the UI to update with a COPY of the list
                OnMonitorUpdated?.Invoke(_sessions.ToList());
            }
        }
    }
}