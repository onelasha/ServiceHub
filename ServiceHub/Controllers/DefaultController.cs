using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;


namespace ServiceHub.Controllers
{
    [Route("")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
        private readonly IConfiguration _configuration;

        public DefaultController(ILogger<DefaultController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: Empty/Default controller
        [HttpGet]
        public async void Get()
        {
            bool isEncrypted = Convert.ToBoolean(_configuration["ConnectionStrings:Encrypted"]);

            if (isEncrypted)
            {
                /*
                 * In order to decrypt using this certificate, IIS_IUSRS has to have access to private key
                 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
                 WinHttpCertCfg.exe -g -c LOCAL_MACHINE\WebHosting -s "webgi.app" -a "HOMEVM10PRO\IIS_IUSRS"

                */


                /*
                X509Store store = new X509Store("WebHosting", StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection foundCertificates =
                    store.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        "7970ca8de61cae007db15dfb01cf6f961397ef02",
                        true);
                store.Close();
                if (foundCertificates.Count == 0)
                    return;

                X509Certificate2 cert = foundCertificates[0];
                */

                string encr = GIxUtils.EncryptString("Data Source=172.22.22.12;Initial Catalog=GI_TEST;User ID=WebGi;Password=P@$$w0rd4W3bG1;Persist Security Info=True;TrustServerCertificate=true;");
                string decr = GIxUtils.DecyptString(encr);

                encr = GIxUtils.EncryptString("Barjakuzu010203");

                //X509Certificate2 cert = new X509Certificate2(@"C:\webgi\webgi-app.pfx");

                /*
                byte[] data = Encrypt(cert);
                byte[] data2 = Decrypt(cert, data);
                string decrString = Encoding.ASCII.GetString(data2);

                string base64Encoded = Convert.ToBase64String(data);
                byte[] data3 = Convert.FromBase64String(base64Encoded);

                byte[] data4 = Decrypt(cert, data3);
                string decrString3 = Encoding.ASCII.GetString(data4);
                */

                //_configuration["ConnectionStrings:DefaultConnection"] = "OK";
            }


            //"<title>Service Hub</title><br/>**** " + _configuration["ConnectionStrings:DefaultConnection"] + "*****<br/>"+

            await Response.WriteAsync("<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "<meta charset=\"UTF - 8\">" +
                "<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1.0\">" +
                "<style>body{font-family: Segoe UI,SegoeUI,Segoe WP,Helvetica Neue,Helvetica,Tahoma,Arial,sans-serif;font-weight: 400;}</style></head>" +
                "<body>" +
                "<center><h1>Service Hub - "+ _configuration["Environmet"] + "</h1><hr><p>Nothing to see here</p></center>" +
                "</body>" +
                "</html>");
        }

        //7970ca8de61cae007db15dfb01cf6f961397ef02

        private X509Certificate2 GetCertificateFromStore(string certName)
        {

            // Get the certificate store for the current user.
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                // Place all certificates in an X509Certificate2Collection object.
                X509Certificate2Collection certCollection = store.Certificates;
                // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
                // currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, true);
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                if (signingCert.Count == 0)
                    return null;
                // Return the first certificate in the collection, has the right name and is current.
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }

        private byte[] Encrypt(X509Certificate2 cert) {
            using (RSA rsa = cert.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formermly just "OAEP" is
                // now OAEP-SHA1.
                //UnicodeEncoding encoding = new UnicodeEncoding();
                string encr = @"Lalalala";
                //Encoding.UTF8.GetBytes(utfString)
                //byte[] data = encoding.GetBytes(encr);
                byte[] data = Encoding.ASCII.GetBytes(encr);
                return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
            }

        }

        private byte[] Decrypt(X509Certificate2 cert, byte[] data)
        {
            // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            using (RSA rsa = cert.GetRSAPrivateKey())
            {
                return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
            }
        }

    }
}