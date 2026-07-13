using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinynetbootServer.Core
{
    public class ServerConfig
    {
        public string ServerIp { get; set; } = "192.168.5.251";
        public string StartIp { get; set; } = "192.168.5.100";
        public string EndIp { get; set; } = "192.168.5.200";
        public int HttpPort { get; set; } = 80;
        public bool IsExamMode { get; set; } = false;
    }
}