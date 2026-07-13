using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinynetbootServer.Core;


namespace TinynetbootServer.Network
{
    public class HttpServer
    {
        public Action<string>? OnLog;
        private readonly HttpListener _listener;
        private readonly string _basePath = AppContext.BaseDirectory;
        private readonly string _serverIp;
        private readonly int _httpPort;
        private readonly BootMonitor _bootMonitor;
        private readonly IPManager _ipManager;
        private readonly Func<bool> _isExamModeCheck;
        private readonly string _examServerUrl;

        // Using your existing private local field for Security Operations
        private readonly SecurityManager _securityManager = new SecurityManager();

        private readonly SeatRegistrationManager _seatManager =
            new SeatRegistrationManager();
        public HttpServer(string ip, int port, BootMonitor bootMonitor, IPManager ipManager, Func<bool> isExamModeCheck, string examServerUrl)
        {
            _serverIp = ip;
            _httpPort = port;
            _bootMonitor = bootMonitor;
            _ipManager = ipManager;
            _isExamModeCheck = isExamModeCheck;
            _examServerUrl = examServerUrl;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://{ip}:{port}/");
        }

        private void Log(string msg) => OnLog?.Invoke($"[HTTP] {msg}");

        public async Task StartAsync(CancellationToken ct)
        {
            try
            {
                _listener.Start();
                Log($"HTTP Active on Port {_httpPort}. Bound to: {_serverIp}");
                while (!ct.IsCancellationRequested)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    _ = Task.Run(() => ProcessRequest(context), ct);
                }
            }
            catch (Exception ex) when (ct.IsCancellationRequested || ex is ObjectDisposedException)
            {
                Log("HTTP Server shut down.");
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
            finally
            {
                if (_listener.IsListening) _listener.Stop();
            }
        }

        private void SendString(HttpListenerContext context, string data, string contentType)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                context.Response.ContentType = contentType;
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Log($"[HTTP] SendString Error: {ex.Message}");
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                string clientIp = request.RemoteEndPoint?.Address.ToString() ?? "Unknown";
                string rawPath = request.Url?.AbsolutePath.TrimStart('/') ?? "";
                string filename = WebUtility.UrlDecode(rawPath);

                // =========================================================================
                // ROUTE 1: Intercept configuration registrations from your HTML Page Form
                // =========================================================================
                if (rawPath.Equals("register-seat", StringComparison.OrdinalIgnoreCase) &&
                    request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    string seatNumber;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        seatNumber = reader.ReadToEnd().Trim().ToUpperInvariant();
                    }

                    if (string.IsNullOrEmpty(seatNumber))
                    {
                        SendTextResponse(response, HttpStatusCode.BadRequest, "Error: Seat number cannot be empty.");
                        return;
                    }

                    // Look up internal hardware map details utilizing the local manager field
                    var clientRecord = _ipManager.GetClientDataByIp(clientIp);

                    if (clientRecord.Key == null || clientRecord.Value == null)
                    {
                        SendTextResponse(response, HttpStatusCode.BadRequest, "Error: Machine identity not found in active DHCP memory pool.");
                        return;
                    }

                    string clientMac = clientRecord.Key;
                    string clientUuid = clientRecord.Value.Uuid;

                    string secretKey = _securityManager.GenerateToken(clientIp);

                    _seatManager.SaveRegistration(
                        seatNumber,
                        clientIp,
                        clientMac,
                        clientUuid,
                        secretKey);

                    _securityManager.AuthorizeHardware(clientMac);

                    if (!string.IsNullOrWhiteSpace(clientUuid))
                    {
                        _securityManager.AuthorizeHardware(clientUuid);
                    }

                    _securityManager.AuthorizeIpSession(clientIp);

                    _ipManager.UpdateStatus(clientMac, "CONFIGURED");

                    var client = _ipManager.GetClientByIp(clientIp);

                    if (client == null)
                    {
                        SendTextResponse(response,
                            HttpStatusCode.BadRequest,
                            "Client not found");
                        return;
                    }

                    string jumpUrl =
     $"{_examServerUrl}?key={Uri.EscapeDataString(client.Key)}";

                    response.Redirect(jumpUrl);
                    return;

                    
                }
                // =========================================================================
                // ROUTE 2: Standard coreetch() operations (boot scripts, kernel assets, files)
                // =========================================================================
                string clientToken = request.QueryString["token"] ?? "";
                    string clientMacParam = request.QueryString["mac"] ?? "";
                    string clientUuidParam = request.QueryString["uuid"] ?? "";
                    bool isExamMode = _isExamModeCheck?.Invoke() ?? false;

                    string subFolder = "examstaytemp";
                    string filePath;

                    bool isSystemFile = filename.Equals("vmlinuz", StringComparison.OrdinalIgnoreCase) ||
                                       filename.Equals("initrd.img", StringComparison.OrdinalIgnoreCase);

                    bool isSquashFs = filename.EndsWith(".squashfs", StringComparison.OrdinalIgnoreCase);

                if (filename.Equals("start", StringComparison.OrdinalIgnoreCase))
                {
                    Log($"START requested from {clientIp}");

                    var client = _ipManager.GetClientByIp(clientIp);

                    if (client == null)
                    {
                        Log($"START FAILED - No client found for IP {clientIp}");

                        response.StatusCode = 403;
                        response.Close();
                        return;
                    }

                    string url =
     $"{_examServerUrl}?key={Uri.EscapeDataString(client.Key)}";

                    Log($"CLIENT IP      : {client.Ip}");
                    Log($"CLIENT NAME    : {client.HostName}");
                    Log($"CLIENT MAC     : {client.Mac}");
                    Log($"CLIENT KEY     : {client.Key}");
                    Log($"REDIRECT URL   : {url}");

                    response.StatusCode = 302;
                    response.RedirectLocation = url;

                    Log("302 Redirect Sent");

                    response.Close();
                    return;
                }
                else if (isSystemFile || isSquashFs)
                    {
                        string cleanFileName = Path.GetFileName(filename);
                        filePath = Path.Combine(_basePath, subFolder, cleanFileName);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(filename))
                        {
                            filename = "config.html";
                        }
                        filePath = Path.Combine(_basePath, filename);
                    }

                    if (filename.Equals("boot.ipxe", StringComparison.OrdinalIgnoreCase))
                    {
                        _securityManager.AuthorizeHardware(clientMacParam);
                        _securityManager.AuthorizeIpSession(clientIp);

                        string token = _securityManager.GenerateToken(clientIp);
                        string script = IpxeScriptBuilder.GenerateSecureScript(_serverIp, _httpPort, token);
                        SendString(context, script, "text/plain");
                        return;
                    }

                    if (isSystemFile)
                    {
                        if (!_securityManager.ValidateToken(clientIp, clientToken))
                        {
                            Log($"[SECURITY] Blocked direct download of {filename} (Invalid Token)");
                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                            return;
                        }
                    }

                    if (isSquashFs && isExamMode)
                    {
                        if (!_securityManager.IsHardwareAuthorized(clientMacParam) && !_securityManager.IsIpAuthorized(clientIp))
                        {
                            Log($"[SECURITY] Denied SquashFS to {clientIp} - Not Authorized for Exam.");
                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                            return;
                        }
                    }

                    if (File.Exists(filePath))

                    {
                    // =========================================================================
                    // ADD THIS FIX BLOCK HERE:
                    // =========================================================================
                    if (!string.IsNullOrWhiteSpace(clientUuidParam) && !string.IsNullOrWhiteSpace(clientMacParam))
                    {
                        var dynamicRecord = _ipManager.GetClientDataByIp(clientIp);
                        if (dynamicRecord.Key != null)
                        {
                            // If the entry exists in RAM but the UUID field is blank, commit it now
                            if (string.IsNullOrEmpty(dynamicRecord.Value.Uuid))
                            {
                                dynamicRecord.Value.Uuid = clientUuidParam;
                            }
                        }
                    }
                  
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            string ext = Path.GetExtension(filePath).ToLower();
                            switch (ext)
                            {
                                case ".html": response.ContentType = "text/html"; break;
                                case ".js": response.ContentType = "application/javascript"; break;
                                case ".css": response.ContentType = "text/css"; break;
                                case ".png": response.ContentType = "image/png"; break;
                                case ".jpg":
                                case ".jpeg": response.ContentType = "image/jpeg"; break;
                                case ".svg": response.ContentType = "image/svg+xml"; break;
                                default: response.ContentType = "application/octet-stream"; break;
                            }
                            response.ContentLength64 = fs.Length;
                            response.StatusCode = (int)HttpStatusCode.OK;
                            fs.CopyTo(response.OutputStream);
                        }

                        Log($"[HTTP] 200 OK: {filename} sent to {clientIp}");

                        string status = isSquashFs ? "ONLINE" : "BOOTING";
                        _bootMonitor.UpdateProgress(clientIp, clientMacParam, status);
                        _ipManager.UpdateStatus(clientMacParam, status);
                    }
                    else
                    {
                        Log($"[HTTP] 404 Not Found: {filename} (Expected at: {filePath})");
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                
            }
            catch (Exception ex)
            {
                Log($"[HTTP] Error: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        public void Stop()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
            }
        }

        private void SendTextResponse(HttpListenerResponse response, HttpStatusCode statusCode, string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                response.StatusCode = (int)statusCode;
                response.ContentType = "text/plain";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Log($"[HTTP] SendTextResponse Error: {ex.Message}");
            }
        }
    }
}