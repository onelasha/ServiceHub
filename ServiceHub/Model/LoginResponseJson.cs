using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class LoginResponseJson
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public string token { get; set; }
        public string userWho { get; set; }
        public string user { get; set; }
        public string email { get; set; }
        public string version { get; set; }
    }
}
