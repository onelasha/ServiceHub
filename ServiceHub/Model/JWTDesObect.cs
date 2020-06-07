using ServiceHub.Controllers;
using ServiceHub.Model;
using System;

namespace ServiceHub
{
    public class JWTDesObect
    {
        public int Expiration { get; set; }
        public LoginRequestJson LoginRequest { get; set; }
    }
}
