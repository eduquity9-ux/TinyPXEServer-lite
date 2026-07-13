using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TinynetbootServer.Core
{
    public class PxeDownloader
    {
        // 1. Points to your PHP Gatekeeper on cPanel
        private const string DownloadUrl = "https://examstay.com/pxefilesystem/download.php?key=9fK2Lm7QxZp4Rw8Nc6UvT1yHs";

        // 2. MUST match your $allowed_agent in the PHP script exactly
        private const string UserAgentKey = "fjbbw346ibbrebi14ebnv";

        private readonly string _targetPath;

        public PxeDownloader()
        {
            string folder = Path.Combine(AppContext.BaseDirectory, "examstaytemp");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            _targetPath = Path.Combine(folder, "filesystem.squashfs");
        }

        public async Task DownloadSquashFsAsync(IProgress<double> progress)
        {
            if (File.Exists(_targetPath)) File.Delete(_targetPath);

            // CRITICAL: AllowAutoRedirect = true allows C# to follow the PHP 'Location' header to S3
            var handler = new HttpClientHandler() { AllowAutoRedirect = true };

            using (var client = new HttpClient(handler))
            {
                // Prevent timeout for the 1.3GB file
                client.Timeout = TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite);

                // Identify as your authorized app
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentKey);

                using (var response = await client.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                    using (var fileStream = new FileStream(_targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var downloadStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[131072]; // 128KB buffer
                        int read;
                        long totalRead = 0;

                        while ((read = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            if (totalBytes != -1)
                                progress?.Report((double)totalRead / totalBytes * 100);
                        }
                    }
                }
            }
        }
    }
}