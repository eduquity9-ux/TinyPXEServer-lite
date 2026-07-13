using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TinynetbootServer.Core
{
    public class SecurityManager
    {
        private readonly Dictionary<string, string> _activeTokens = new();
        private readonly Dictionary<string, DateTime> _authorizedHardware = new();
  
        private readonly Dictionary<string, DateTime> _authorizedIPs = new();

        public string GenerateToken(string ip)
        {
            string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
            lock (_activeTokens) { _activeTokens[ip] = token; }
            return token;
        }

        public bool ValidateToken(string ip, string token)
        {
            lock (_activeTokens)
            {
                return _activeTokens.TryGetValue(ip, out var storedToken) && storedToken == token;
            }
        }

        public void AuthorizeHardware(string hardwareId)
        {
            if (string.IsNullOrWhiteSpace(hardwareId)) return;
            lock (_authorizedHardware)
            {
                _authorizedHardware[hardwareId.ToLower()] = DateTime.Now.AddMinutes(20);
            }
        }

        public bool IsHardwareAuthorized(string hardwareId)
        {
            if (string.IsNullOrWhiteSpace(hardwareId)) return false;
            lock (_authorizedHardware)
            {
                if (_authorizedHardware.TryGetValue(hardwareId.ToLower(), out var expiry))
                {
                    return DateTime.Now < expiry;
                }
            }
            return false;
        }

        // --- NEW: IP SESSION LOGIC ---
        public void AuthorizeIpSession(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return;
            lock (_authorizedIPs)
            {
                _authorizedIPs[ip] = DateTime.Now.AddMinutes(20);
            }
        }

        public bool IsIpAuthorized(string ip)
        {
            lock (_authorizedIPs)
            {
                return _authorizedIPs.TryGetValue(ip, out var expiry) && DateTime.Now < expiry;
            }
        }
    }
}