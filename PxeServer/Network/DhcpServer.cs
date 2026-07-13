using System.Net;
using System.Net.Sockets;
using System.Text;
using TinynetbootServer.Core;


namespace TinynetbootServer.Network
{
    public class DhcpServer
    {
        public Action<string>? OnLog;
        
        private readonly UdpClient _listener;
        private readonly IPManager _ipManager;
        private readonly string _serverIp;
        private readonly int _httpPort;
        private readonly bool _isExamMode;
        private readonly BootMonitor _bootMonitor;


        public DhcpServer(IPManager ipManager, string serverIp, int httpPort, bool isExamMode, BootMonitor bootMonitor)
        {
            _ipManager = ipManager;
            _serverIp = serverIp;
            _httpPort = httpPort;
            _isExamMode = isExamMode;
            _bootMonitor = bootMonitor;
            _listener = new UdpClient();
            _listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // Bind to the Server IP on Port 67 (Standard DHCP Port)
            _listener.Client.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), 67));
        }

        private void Log(string msg) => OnLog?.Invoke($"[DHCP] {msg}");

        public async Task StartAsync(CancellationToken ct)
        {
            Log($"DHCP Engine Online. Detecting BIOS/UEFI architectures...");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var result = await _listener.ReceiveAsync(ct);
                    var packet = DhcpPacket.Parse(result.Buffer);
                    if (!packet.Options.ContainsKey(53)) continue;

                    string mac = packet.GetMacAddress();
                    // We add string.Empty in the middle to satisfy the (mac, uuid, isExamMode) requirement
                    var client =
                        _ipManager.GetClientByMac(mac);

                    if (client == null)
                    {
                        Log($"MAC not registered: {mac}");
                        continue;
                    }

                    string clientIp = client.Ip;
                    if (string.IsNullOrEmpty(clientIp)) continue;
                    _bootMonitor.UpdateProgress(clientIp, mac, "BOOTING");
                    byte msgType = packet.Options[53][0];
                    bool isIPxe = packet.Options.ContainsKey(77) &&
                                 Encoding.ASCII.GetString(packet.Options[77]).Contains("iPXE");

                    if (msgType == 1) await SendResponse(packet, clientIp, 2, isIPxe);
                    else if (msgType == 3) await SendResponse(packet, clientIp, 5, isIPxe);
                }
                catch (Exception ex) { Log($"Error: {ex.Message}"); }
            }
        }

        private async Task SendResponse(DhcpPacket request, string clientIp, byte type, bool isIPxe)
        {
            var res = new DhcpPacket
            {
                Op = 2,
                Xid = request.Xid,
                Chaddr = request.Chaddr,
                Yiaddr = IPAddress.Parse(clientIp).GetAddressBytes(),
                Siaddr = IPAddress.Parse(_serverIp).GetAddressBytes()
            };

            res.Options[53] = new byte[] { type };
            res.Options[54] = IPAddress.Parse(_serverIp).GetAddressBytes();
            res.Options[1] = new byte[] { 255, 255, 255, 0 };
            res.Options[3] = IPAddress.Parse(_serverIp).GetAddressBytes();
            res.Options[6] = IPAddress.Parse(_serverIp).GetAddressBytes();
            res.Options[66] = IPAddress.Parse(_serverIp).GetAddressBytes();

            if (!isIPxe)
            {
                // DYNAMIC ARCHITECTURE DETECTION (Option 93)
                int arch = 0;
                if (request.Options.ContainsKey(93))
                {
                    byte[] d = request.Options[93];
                    arch = d.Length == 2 ? d[1] : d[0];
                }

                string bootFile = arch switch
                {
                    0 => "undionly.kpxe",      // Legacy BIOS
                    6 => "ipxe.efi",           // EFI x86
                    7 => "snponly.efi",        // EFI x64
                    9 => "snponly.efi",        // EFI BC
                    11 => "boota64.efi",       // EFI ARM64
                    _ => "undionly.kpxe"
                };

                res.Options[67] = Encoding.ASCII.GetBytes(bootFile + "\0");
                Log($"[DETECT] Arch {arch} -> Serving {bootFile}");
            }
            else
            {
                // Phase 2: Redirect to HTTP iPXE Script
                string scriptUrl = $"http://examstay.local:{_httpPort}/boot.ipxe\0";
                res.Options[67] = Encoding.ASCII.GetBytes(scriptUrl);
            }

            byte[] data = res.ToBytes();
            await _listener.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 68));
        }

        public void Stop()
        {
            try
            {
                _listener.Close();
                _listener.Dispose();
            }
            catch { /* Cleanup ignored */ }
        }
    }
}