using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TinynetbootServer.Core
{
    public class IPManager
    {
        private readonly string _subnetPrefix;
        private int _rangeStart; // Modified to allow flexible range adjustments
        private int _rangeEnd;   // Modified to allow flexible range adjustments
        private int _currentOffset;
       
        public class ClientEntry { public string Ip = ""; public string Status = ""; public string Uuid { get; set; } = ""; }
        private readonly ConcurrentDictionary<string, ClientEntry> _clients = new();
        private readonly Dictionary<string, ClientRecord> _staticClients = new();
        public IPManager(string startIp, string endIp)
        {
            try
            {
                int lastDot = startIp.LastIndexOf('.');
                _subnetPrefix = startIp.Substring(0, lastDot + 1);
                _rangeStart = int.Parse(startIp.Substring(lastDot + 1));
                _rangeEnd = int.Parse(endIp.Substring(endIp.LastIndexOf('.') + 1));
                _currentOffset = _rangeStart;

               
            }
            catch (Exception)
            {
                _subnetPrefix = "192.168.1.";
                _rangeStart = 100;
                _rangeEnd = 200;
                _currentOffset = _rangeStart;
            }
        }
        public void LoadClientsCsv()
        {
            string file =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "config",
                    "clients.csv");

            if (!File.Exists(file))
                return;

            foreach (string line in File.ReadLines(file).Skip(1))
            {
                string[] p = line.Split(',');

                if (p.Length < 4)
                    continue;

                string mac =
                    p[0]
                    .Replace(":", "")
                    .Replace("-", "")
                    .ToUpperInvariant();

                _staticClients[mac] = new ClientRecord
                {
                    Mac = mac,
                    Ip = p[1].Trim(),
                    HostName = p[2].Trim(),
                    Key = p[3].Trim()
                };
            }
        }
        public ClientRecord? GetClientByIp(string ip)
        {
            return _staticClients.Values
                .FirstOrDefault(x => x.Ip == ip);
        }

        public ClientRecord? GetClientByMac(string mac)
        {
            string cleanMac = mac
                .Replace(":", "")
                .Replace("-", "")
                .Replace(" ", "")
                .ToUpperInvariant();

            if (_staticClients.TryGetValue(cleanMac, out var client))
                return client;

            return null;
        }

        public string GetIpForMac(string mac, string uuid, bool isExamMode)
        {
            string cleanMac = mac.Replace(":", "").Replace("-", "").ToUpperInvariant();

            if (_clients.TryGetValue(cleanMac, out var entry))
            {
                if (!isExamMode && string.IsNullOrEmpty(entry.Uuid) && !string.IsNullOrEmpty(uuid))
                {
                    entry.Uuid = uuid;
                }
                return entry.Ip;
            }

            if (isExamMode) return string.Empty;

            if (_currentOffset <= _rangeEnd)
            {
                string newIp = $"{_subnetPrefix}{_currentOffset++}";
                var newEntry = new ClientEntry
                {
                    Ip = newIp,
                    Status = "PENDING",
                    Uuid = uuid
                };
                _clients.TryAdd(cleanMac, newEntry);
                return newIp;
            }
            return string.Empty;
        }

        public void UpdateStatus(string mac, string status)
        {
            string cleanMac = mac.Replace(":", "").Replace("-", "").ToUpperInvariant();
            if (_clients.TryGetValue(cleanMac, out var entry))
            {
                entry.Status = status;

                // FIXED: Automatically saving to config/leases.csv during early boot stages 
                // ("BOOTING" / "ONLINE") has been completely removed. 
                // Hardware profiles now remain exclusively in RAM.
            }
        }

        public KeyValuePair<string, ClientEntry> GetClientDataByIp(string ip)
        {
            return _clients.FirstOrDefault(x => x.Value.Ip == ip);
        }

        public List<(string Mac, string Ip, string Status, string Uuid)> GetAllClients()
            => _clients.Select(x => (x.Key, x.Value.Ip, x.Value.Status, x.Value.Uuid)).ToList();

     

       
        public void UpdateRange(string startIp, string endIp)
        {
            try
            {
                int lastDot = startIp.LastIndexOf('.');
                int newStart = int.Parse(startIp.Substring(lastDot + 1));
                int newEnd = int.Parse(endIp.Substring(endIp.LastIndexOf('.') + 1));

                if (newStart != _rangeStart || newEnd != _rangeEnd)
                {
                    _rangeStart = newStart;
                    _rangeEnd = newEnd;
                }
            }
            catch { /* Keep existing range if parsing fails */ }
        }
        public void AddOrUpdateClient(string ip, string mac, string uuid, string status)
        {
            // Normalize to uppercase without delimiters to match your internal dictionary strategy
            string cleanMac = mac.Replace(":", "").Replace("-", "").Replace(" ", "").ToUpperInvariant();

            if (string.IsNullOrEmpty(cleanMac)) return;

            var entry = new ClientEntry
            {
                Ip = ip,
                Status = string.IsNullOrWhiteSpace(status) ? "REGISTERED" : status.ToUpperInvariant(),
                Uuid = uuid
            };

            // Thread-safe update or insertion into memory
            _clients[cleanMac] = entry;

            // Track the current offset so new DHCP leases don't overlap with loaded IPs
            try
            {
                int lastDot = ip.LastIndexOf('.');
                if (lastDot != -1)
                {
                    int lastPart = int.Parse(ip.Substring(lastDot + 1));
                    if (lastPart >= _currentOffset)
                    {
                        _currentOffset = lastPart + 1;
                    }
                }
            }
            catch { /* Safe fallback if IP formatting is custom or non-standard */ }

          
        }
        public bool RemoveClient(string mac)
        {
            string cleanMac = mac.Replace(":", "").Replace("-", "").ToUpperInvariant();
            if (_clients.TryRemove(cleanMac, out _))
            {
             
                return true;
            }
            return false;
        }
        public class ClientRecord
        {
            public string Mac { get; set; } = "";
            public string Ip { get; set; } = "";
            public string HostName { get; set; } = "";
            public string Key { get; set; } = "";
        }
    }
}