using System.Text;

namespace TinynetbootServer.Core
{
    public static class IpxeScriptBuilder
    {
        public static string GenerateSecureScript(string ip, int port, string token)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#!ipxe");

            string baseUrl = $"http://examstay.local:{port}";
            string tempFolderUrl = $"{baseUrl}/examstaytemp";

            string mac = "${net0/mac}";
            string uuid = "${uuid}";

            // Use a CLEAN URL for the 'fetch' parameter
            //  sb.AppendLine($"kernel {baseUrl}/vmlinuz?token={token}&mac={mac}&uuid={uuid} " +
            //       $"boot=live components ip=dhcp fetch={tempFolderUrl}/filesystem.squashfs loglevel=3 quiet");

            sb.AppendLine($"kernel {baseUrl}/vmlinuz?token={token}&mac={mac}&uuid={uuid} " +
        $"boot=live components ip=dhcp " +
        $"serverip={ip} serverport={port} " +
        $"fetch={tempFolderUrl}/filesystem.squashfs loglevel=3 quiet");

            sb.AppendLine($"initrd {baseUrl}/initrd.img?token={token}&mac={mac}&uuid={uuid}");

            sb.AppendLine("boot");
            return sb.ToString();
        }
    }
}