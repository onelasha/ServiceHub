using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceHub.Controllers
{
    public class GIUserAccount
    {
    }

    public class LoginRequestJson
    {
        public string apiKey { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
    }

    public class LoginResponseJson
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public string token { get; set; }
    }


    public class WebGiBaseController : ControllerBase
    {
        public LoginRequestJson _loginRequest;

        public WebGiBaseController()
        {
            _loginRequest = new LoginRequestJson();
        }

        public void DecodeLoginRequest(IConfiguration configuration)
        {
            string secret = configuration["JWTSecret"];
            string token = Request.Headers["X-WebGI-Authentication"];

            var json = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

            dynamic jbody = JsonConvert.DeserializeObject(json);
            foreach (dynamic item in jbody)
            {
                string name = item.Name;
                string val = item.Value.ToString();
                switch (name)
                {
                    case "apiKey":
                        _loginRequest.apiKey= val;
                        break;
                    case "username":
                        _loginRequest.username = val;
                        break;
                    case "password":
                        _loginRequest.password = val;
                        break;
                    case "salt":
                        _loginRequest.salt = val;
                        break;
                }
            }

        }
        public string DecodeConnectionsString(IConfiguration configuration)
        {

            string dbServer = configuration["DBServer"];
            string dbName = configuration["DBName"];
            DecodeLoginRequest(configuration);
            return $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={_loginRequest.username};Password={_loginRequest.password};TrustServerCertificate=true;";
        }
    }
}
