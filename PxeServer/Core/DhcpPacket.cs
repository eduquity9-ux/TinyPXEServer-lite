using System.Net;

namespace TinynetbootServer.Core
{
    public class DhcpPacket
    {
        public byte Op { get; set; }
        public byte[] Xid { get; set; } = new byte[4];
        public byte[] Flags { get; set; } = new byte[2];  // offset 10 — broadcast flag
        public byte[] Yiaddr { get; set; } = new byte[4];  // offset 16 — assigned client IP
        public byte[] Siaddr { get; set; } = new byte[4];  // offset 20 — next-server (TFTP)
        public byte[] Giaddr { get; set; } = new byte[4];  // offset 24 — relay agent IP
        public byte[] Chaddr { get; set; } = new byte[16]; // offset 28 — client MAC

        public Dictionary<byte, byte[]> Options { get; set; } = new();

        public static DhcpPacket Parse(byte[] data)
        {
            var p = new DhcpPacket();
            if (data.Length < 240) return p;

            p.Op = data[0];
            Array.Copy(data, 4, p.Xid, 0, 4);
            Array.Copy(data, 10, p.Flags, 0, 2); // broadcast flag
            Array.Copy(data, 24, p.Giaddr, 0, 4); // relay agent
            Array.Copy(data, 28, p.Chaddr, 0, 6); // MAC (only first 6 bytes matter)

            int offset = 240; // skip header + magic cookie
            while (offset < data.Length)
            {
                byte tag = data[offset++];
                if (tag == 0xFF) break;  // End option
                if (tag == 0x00) continue; // Pad option
                if (offset >= data.Length) break;
                byte len = data[offset++];
                if (offset + len > data.Length) break;
                byte[] val = new byte[len];
                Array.Copy(data, offset, val, 0, len);
                p.Options[tag] = val;
                offset += len;
            }

            return p;
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[1024];

            data[0] = Op;
            data[1] = 1;    // htype: Ethernet
            data[2] = 6;    // hlen: MAC address length
            data[3] = 0;    // hops

            Array.Copy(Xid, 0, data, 4, 4); // transaction ID
            Array.Copy(Flags, 0, data, 10, 2); // CRITICAL: echo broadcast flag — UEFI requires this
            Array.Copy(Yiaddr, 0, data, 16, 4); // your (client) IP address
            Array.Copy(Siaddr, 0, data, 20, 4); // next-server IP (TFTP server)
            Array.Copy(Giaddr, 0, data, 24, 4); // relay agent IP — echo back
            Array.Copy(Chaddr, 0, data, 28, 6); // client MAC address

            // Also write boot filename into the 'file' field (offset 108, 128 bytes)
            // Some UEFI firmware reads the filename from here, not Option 67
            if (Options.TryGetValue(67, out byte[]? bootFile))
            {
                int copyLen = Math.Min(bootFile.Length, 127);
                Array.Clear(data, 108, 128);
                Array.Copy(bootFile, 0, data, 108, copyLen);
                // data[108 + copyLen] stays 0 = null terminator
            }

            // DHCP magic cookie
            data[236] = 0x63;
            data[237] = 0x82;
            data[238] = 0x53;
            data[239] = 0x63;

            // Write options
            int offset = 240;
            foreach (var opt in Options)
            {
                if (offset + opt.Value.Length + 2 >= data.Length - 1) break;
                data[offset++] = opt.Key;
                data[offset++] = (byte)opt.Value.Length;
                Array.Copy(opt.Value, 0, data, offset, opt.Value.Length);
                offset += opt.Value.Length;
            }

            data[offset] = 0xFF; // End option
            return data;
        }

        public string GetMacAddress() =>
            BitConverter.ToString(Chaddr, 0, 6).Replace("-", ":");
    }
}