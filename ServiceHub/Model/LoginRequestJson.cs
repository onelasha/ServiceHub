using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class LoginRequestJson
    {
        public string apiKey { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string version { get; set; }
    }
}
