    using System.Net;
    using System.Net.Sockets;

    namespace TinynetbootServer.Network
    {
        public class DnsServer
        {
            public Action<string>? OnLog;
            private readonly UdpClient _udpListener;
            private readonly string _serverIp;
            private readonly string _domain = "examstay.local";

            public DnsServer(string serverIp)
            {
                _serverIp = serverIp;
                _udpListener = new UdpClient();
                _udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // Bind ONLY to the server IP you are using for the exam network
                // This way it won't fight with ICS on other IPs
                _udpListener.Client.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), 53));
            }

            public async Task StartAsync(CancellationToken ct)
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        // If the UI calls Stop(), this line will throw an ObjectDisposedException
                        var result = await _udpListener.ReceiveAsync(ct);

                        if (result.Buffer.Length > 12)
                        {
                            byte[] response = BuildResponse(result.Buffer);
                            await _udpListener.SendAsync(response, response.Length, result.RemoteEndPoint);
                        }
                    }
                }
                catch (Exception) when (ct.IsCancellationRequested)
                {
                    // This is a clean exit, we don't need to log an error
                }
                finally
                {
                    _udpListener.Close();
                }
            }

            public void Stop() => _udpListener.Dispose(); // Dispose is cleaner than Close

            private byte[] BuildResponse(byte[] request)
            {
                // This creates a basic DNS "A Record" response
                byte[] response = new byte[request.Length + 16];
                Array.Copy(request, 0, response, 0, request.Length);

                // Set Response Flags (Standard Query Response)
                response[2] = 0x81; response[3] = 0x80;
                // Set Answer Count to 1
                response[7] = 0x01;

                int offset = request.Length;
                // Name Pointer (points to the domain name in the question)
                response[offset++] = 0xc0; response[offset++] = 0x0c;
                // Type A (Host Address)
                response[offset++] = 0x00; response[offset++] = 0x01;
                // Class IN (Internet)
                response[offset++] = 0x00; response[offset++] = 0x01;
                // TTL (Time to Live) - 1 Hour
                response[offset++] = 0x00; response[offset++] = 0x00; response[offset++] = 0x0e; response[offset++] = 0x10;
                // Data Length (4 bytes for IP)
                response[offset++] = 0x00; response[offset++] = 0x04;

                // The IP Address bytes
                byte[] ipBytes = IPAddress.Parse(_serverIp).GetAddressBytes();
                Array.Copy(ipBytes, 0, response, offset, 4);

                return response;
            }

       
        }
    }