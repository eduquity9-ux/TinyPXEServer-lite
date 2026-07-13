using System;
using System.IO;
using System.Net;
using Tftp.Net;

namespace TinynetbootServer.Network
{
    public class TftpServer : IDisposable
    {
        public Action<string>? OnLog;

        // Use full path to avoid conflict with THIS class name
        private readonly Tftp.Net.TftpServer _innerServer;
        private readonly string _basePath = AppContext.BaseDirectory;
        private bool _disposedValue;

        private void Log(string msg) => OnLog?.Invoke($"[TFTP] {msg}");

        public TftpServer(string serverIp)
        {
            // Initialize the library's server
            _innerServer = new Tftp.Net.TftpServer(IPAddress.Parse(serverIp), 69);

            // FIX: The OnError delegate only takes 1 argument (the error object)
            _innerServer.OnReadRequest += Server_OnReadRequest;
            _innerServer.OnError += (error) => Log($"[SERVER ERROR] {error}");
        }

        // Renamed to Start() to match standard library patterns
        public void Start()
        {
            _innerServer.Start();
            Log($"Engine Online. Root: {_basePath}");
        }

        private void Server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            string fullPath = Path.GetFullPath(Path.Combine(_basePath, transfer.Filename));

            if (!fullPath.StartsWith(_basePath, StringComparison.InvariantCultureIgnoreCase))
            {
                Log($"[REJECTED] Path violation: {transfer.Filename}");
                transfer.Cancel(TftpErrorPacket.AccessViolation);
                return;
            }

            if (!File.Exists(fullPath))
            {
                Log($"[NOT FOUND] {transfer.Filename}");
                transfer.Cancel(TftpErrorPacket.FileNotFound);
                return;
            }

            transfer.OnFinished += (t) => Log($"[DONE] Sent {t.Filename} to {client}");
            transfer.OnError += (t, error) => Log($"[FAIL] {t.Filename}: {error}");

            try
            {
                var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                transfer.Start(stream);
            }
            catch (Exception ex)
            {
                Log($"[IO ERROR] {ex.Message}");
                transfer.Cancel(TftpErrorPacket.AccessViolation);
            }
        }

        // Re-adding the Stop method for your UI buttons to call

        // Add this so Form1.cs can find the 'Stop' method
        public void Stop()
        {
            // Simply call Dispose() internally to shut down the server
            Dispose();
            Log("Engine Offline.");
        }
        public void Dispose()
        {
            if (!_disposedValue)
            {
                _innerServer.Dispose();
                _disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}