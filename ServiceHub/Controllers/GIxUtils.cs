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
using ServiceHub.Model;

namespace ServiceHub.Controllers
{
    public static class GIxUtils 
    {
        static private void DecodeLoginRequest(IConfiguration configuration, ref LoginRequestJson loginRequest, string token, string version)
        {
            string secret = configuration["JWTSecret"];
            //string token = Request.Headers["X-WebGI-Authentication"];
            //string version = Request.Headers["X-WebGI-Version"];

            var json = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);
            JWTDesObect desObject = JsonConvert.DeserializeObject<JWTDesObect>(json);

            if (desObject == null || desObject.LoginRequest == null)
                throw new Exception("Not valid");

            if (desObject.LoginRequest.version != version)
                throw new Exception("Invalid app version! Log off and log back again");


            loginRequest = desObject.LoginRequest;
        }

        static public string DecodeConnectionString(IConfiguration configuration, ref LoginRequestJson loginRequest, string token, string version)
        {
            string dbServer = configuration["DBServer"];
            string dbName = configuration["DBName"];
            DecodeLoginRequest(configuration, ref loginRequest, token, version);
            return $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={loginRequest.username};Password={loginRequest.password};TrustServerCertificate=true;";
        }
    }
}
