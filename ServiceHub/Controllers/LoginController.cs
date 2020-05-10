using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceHub.Controllers
{
    
    [Route("authentication/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private bool dbCheckAPIKey(ref LoginRequestJson req, ref LoginResponseJson resp)
        {
            bool rezult = false;

            try
            {
                //string username = "GI_ADM";
                //string password = "asdASD123";
                string dbServer = _configuration["DBServer"];
                string dbName = _configuration["DBName"];
                string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
                string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();
                string connectionString = $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={req.username};Password={req.password};TrustServerCertificate=true;";


                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "dbo.[usp_WebGI_ChekAPIKey]";
                        sqlCommand.Parameters.AddWithValue("@APIKey", req.apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            if (recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("IsOk")]) != System.DBNull.Value) rezult = (bool)value;
                            }
                            recordSet.Close();
                            recordSet.Dispose();
                        }
                    }

                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            if (!rezult)
                throw new Exception("Unauthorized access detected, Invalid access token.");
            return rezult;
        }
        private bool dbIssueSessionToken(ref LoginRequestJson req, ref LoginResponseJson resp) 
        {
            string dbServer = _configuration["DBServer"];
            string dbName = _configuration["DBName"];
            string username = req.username;
            string password = req.password;
            string uniqueID = string.Empty;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();
            string connectionString = $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={username};Password={password};TrustServerCertificate=true;";


            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "dbo.[usp_WebGI_IssueSessionToken]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            if (recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("IsOk")]) != System.DBNull.Value) resp.success = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("UniqueID")]) != System.DBNull.Value) req.salt = (string)value;
                            }
                            recordSet.Close();
                            recordSet.Dispose();
                        }
                    }

                    /////
                    /// JWT Base64 user credentials as sessionvarialbelHas + guid from DB
                    
                    var token = new JwtBuilder()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(_configuration["JWTSecret"])
                      //.AddClaim("expiration", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()) // 
                      .AddClaim("LoginRequestJson", req)
                      .Encode();

                    Console.WriteLine(token);
                    resp.token = token;

                    sqlConnection.Close();
                    sqlConnection.Dispose();


                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!resp.success)
                throw new Exception("Authentication issue detected - Invalid SessionToken");
            return resp.success;
        }


        // GET: Login
        [HttpPost]
        public async Task<JsonResult> Login()
        {
            LoginRequestJson reqObj = new LoginRequestJson();
            LoginResponseJson respObj = new LoginResponseJson()
            {
                success = true,
                message = "Ok",
                code = 0,
                token = string.Empty
            };


            try
            {
                //Request.Headers["Authorization"]
                string bearer = Request.Headers["Authorization"];
                if (Request == null || Request.Headers == null || Request.Headers.Count == 0 || string.IsNullOrWhiteSpace(Request.Headers["Authorization"]) == true)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    object respojseObj_MissingBearer = new
                    {
                        success = false,
                        message = "Unauthorized",
                        code = -401,
                        token = ""
                    };
                    return new JsonResult(respojseObj_MissingBearer);
                }
                string[] bearers = bearer.Split("Bearer ");
                if (bearers == null || bearers.Length != 2)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    object respojseObj_MissingBearer = new
                    {
                        success = false,
                        message = "Unauthorized - Missing Bearer Token",
                        code = -401,
                        token = ""
                    };
                    return new JsonResult(respojseObj_MissingBearer);
                }

                using (var reader = new StreamReader(Request.Body))
                {
                    string body = string.Empty;
                    body = await reader.ReadToEndAsync();
                    dynamic jbody = JsonConvert.DeserializeObject(body);
                    foreach (dynamic item in jbody)
                    {
                        //int rowCount = 0;
                        //foreach (dynamic col in rows)
                        {
                            string name = item.Name;
                            string val = item.Value.ToString();
                            switch (name)
                            { 
                               case  "username":
                                    reqObj.username = val;
                                    break;
                                case "password":
                                    reqObj.password = val;
                                    break;
                            }
                        }
                    }
                }

                if (reqObj == null || string.IsNullOrWhiteSpace(reqObj.username) == true || string.IsNullOrWhiteSpace(reqObj.password))
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    object respojseObj_MissingBearer = new
                    {
                        success = false,
                        message = "Unauthorized - No credentials provided",
                        code = -401,
                        token = ""
                    };
                    return new JsonResult(respojseObj_MissingBearer);
                }

                reqObj.apiKey = bearers[1];
                dbCheckAPIKey(ref reqObj, ref respObj);
                dbIssueSessionToken(ref reqObj, ref respObj);

            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                object respojseObj_CheckBearer = new
                {
                    success = false,
                    message = $"Unauthorized - {ex.Message}",
                    code = -401,
                    token = ""
                };
                return new JsonResult(respojseObj_CheckBearer);
            }


            return new JsonResult(respObj);
        }

        //// GET: Login/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: Login
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: Login/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}
            
        //// DELETE: ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
