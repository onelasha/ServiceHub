using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using JWT.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceHub.Model;

namespace ServiceHub.Controllers
{
    public static class GIxUtils 
    {
        static public void Log(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        static public string EncryptString(string string2Encrypt)
        {
            string enryptedString = string.Empty;
            /*
                * In order to decrypt using this certificate, IIS_IUSRS has to have access to private key
                * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
                WinHttpCertCfg.exe -g -c LOCAL_MACHINE\WebHosting -s "webgi.app" -a "HOMEVM10PRO\IIS_IUSRS"

            */
            X509Store store = new X509Store("WebHosting", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection foundCertificates =
                store.Certificates.Find(
                    X509FindType.FindByIssuerName,
                    "Go Daddy Secure Certificate Authority - G2",
                    //X509FindType.FindByThumbprint,
                    //"7970ca8de61cae007db15dfb01cf6f961397ef02",
                    true);
            store.Close();
            if (foundCertificates.Count == 0)
                return "";
            X509Certificate2 cert = foundCertificates[0]; //X509Certificate2 cert = new X509Certificate2(@"C:\webgi\webgi-app.pfx");
            using (RSA rsa = cert.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formermly just "OAEP" is
                // now OAEP-SHA1.
                byte[] data = Encoding.ASCII.GetBytes(string2Encrypt);
                byte[] data2 = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                enryptedString = Convert.ToBase64String(data2);
            }
            return enryptedString;
        }
        static public string DecyptString(string encryptedString)
        {
            string decryptedString = string.Empty;

            /*
                * In order to decrypt using this certificate, IIS_IUSRS has to have access to private key
                * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
                WinHttpCertCfg.exe -g -c LOCAL_MACHINE\WebHosting -s "webgi.app" -a "HOMEVM10PRO\IIS_IUSRS"

            */
            X509Store store = new X509Store("WebHosting", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection foundCertificates =
                store.Certificates.Find(
                    X509FindType.FindByIssuerName,
                    "Go Daddy Secure Certificate Authority - G2",
                    //X509FindType.FindByThumbprint,
                    //"7970ca8de61cae007db15dfb01cf6f961397ef02",
                    true);
            store.Close();
            if (foundCertificates.Count == 0)
                return "";
            byte[] data = Convert.FromBase64String(encryptedString);
            X509Certificate2 cert = foundCertificates[0]; //X509Certificate2 cert = new X509Certificate2(@"C:\webgi\webgi-app.pfx");
            using (RSA rsa = cert.GetRSAPrivateKey())
            {
                byte[] data2 = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
                decryptedString = Encoding.ASCII.GetString(data2);
            }
            return decryptedString;
        }
        static private void DecodeLoginRequest(IConfiguration configuration, ref LoginRequestJson loginRequest, string token, string version, string connectionString)
        {
            string secret = DecyptString(configuration["JWTSecretEncypted"]);

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
            if (string.IsNullOrWhiteSpace(connectionString))
                return;



        }
        static public string DecodeConnectionString(IConfiguration configuration, ref LoginRequestJson loginRequest, string token, string version)
        {
            //string dbServer = configuration["DBServer"];
            //string dbName = configuration["DBName"];
            bool isEncrypted = Convert.ToBoolean(configuration["ConnectionStrings:Encrypted"]);
            string connectionString = configuration["ConnectionStrings:ConnStr"];

            if (isEncrypted == true)
                connectionString = DecyptString(connectionString);
            if (!string.IsNullOrWhiteSpace(token) && token != "null" )
                DecodeLoginRequest(configuration, ref loginRequest, token, version, connectionString);


            return connectionString;
            //return $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={loginRequest.username};Password={loginRequest.password};TrustServerCertificate=true;";
        }

    }
}
