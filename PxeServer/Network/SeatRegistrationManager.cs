using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TinynetbootServer.Core
{
    public class SeatRegistrationManager
    {
        private readonly string _filePath =
            Path.Combine(AppContext.BaseDirectory, "config", "leases.csv");

        public class SeatRecord
        {
            public string SeatNumber { get; set; } = "";
            public string Ip { get; set; } = "";
            public string Mac { get; set; } = "";
            public string Uuid { get; set; } = "";
            public string Token { get; set; } = "";
            public DateTime RegisteredAt { get; set; }
        }

        // Standardized helper to format any raw string into a clean colon MAC address
        private string FormatMacWithColons(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac)) return "";
            string clean = mac.Replace(":", "").Replace("-", "").Replace(" ", "").Trim().ToUpperInvariant();
            if (clean.Length != 12) return clean;
            return string.Join(":", Enumerable.Range(0, 6).Select(i => clean.Substring(i * 2, 2)));
        }

        public void SaveRegistration(
             string seatNumber,
             string ip,
             string mac,
             string uuid,
             string token)
        {
            try
            {
                List<SeatRecord> records = LoadRegistrations();

                // Normalize incoming MAC address to 52:54:00:12:34:56
                string formattedMac = FormatMacWithColons(mac);

                // STRICTOR UNIQUENESS PURGE
                records.RemoveAll(x =>
                    x.SeatNumber.Equals(seatNumber, StringComparison.OrdinalIgnoreCase) ||
                    FormatMacWithColons(x.Mac).Equals(formattedMac, StringComparison.OrdinalIgnoreCase) ||
                    x.Ip.Equals(ip, StringComparison.OrdinalIgnoreCase)
                );

                // Add the clean single record with colon formatting preserved
                records.Add(new SeatRecord
                {
                    SeatNumber = seatNumber,
                    Ip = ip,
                    Mac = formattedMac, // <-- Saved as 52:54:00:12:34:56
                    Uuid = uuid,
                    Token = token,
                    RegisteredAt = DateTime.Now
                });

                SaveAll(records);
            }
            catch
            {
                // Fail-safe fallback to prevent crashing the HTTP context thread
            }
        }

        public List<SeatRecord> LoadRegistrations()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<SeatRecord>();

                string encrypted = File.ReadAllText(_filePath);
                string csv = EncryptionHelper.Decrypt(encrypted);

                var result = new List<SeatRecord>();

                foreach (string line in csv.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] p = line.Split(',');

                    if (p.Length < 6)
                        continue;

                    result.Add(new SeatRecord
                    {
                        SeatNumber = p[0],
                        Ip = p[1],
                        Mac = FormatMacWithColons(p[2]), // Normalize on read
                        Uuid = p[3],
                        Token = p[4],
                        RegisteredAt = DateTime.Parse(p[5])
                    });
                }

                return result;
            }
            catch
            {
                return new List<SeatRecord>();
            }
        }

        private void SaveAll(List<SeatRecord> records)
        {
            string? dir = Path.GetDirectoryName(_filePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            string csv = string.Join(
                Environment.NewLine,
                records.Select(r =>
                    $"{r.SeatNumber}," +
                    $"{r.Ip}," +
                    $"{FormatMacWithColons(r.Mac)}," + // Safe export verification
                    $"{r.Uuid}," +
                    $"{r.Token}," +
                    $"{r.RegisteredAt:s}"
                ));

            string encrypted = EncryptionHelper.Encrypt(csv);
            File.WriteAllText(_filePath, encrypted);
        }

        public string GetSeatNumberByMac(string mac)
        {
            var records = LoadRegistrations();
            string searchMac = FormatMacWithColons(mac);

            var record = records.FirstOrDefault(x =>
                FormatMacWithColons(x.Mac).Equals(searchMac, StringComparison.OrdinalIgnoreCase));

            return record?.SeatNumber ?? "";
        }
    }
}