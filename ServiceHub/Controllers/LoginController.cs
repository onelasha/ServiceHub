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
using ServiceHub.Model;

namespace ServiceHub.Controllers
{
    
    [Route("authentication/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private LoginRequestJson _loginRequest;
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration configuration/*, IHttpContextAccessor accessor*/)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private bool dbCheckAPIKey(ref LoginRequestJson req, ref LoginResponseJson resp)
        {
            bool rezult = false;

            try
            {
                string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
                string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();
                //string localHost = HttpContext.Features.Get()?.RemoteIpAddress?.ToString();
                //var a = HttpContext.Features.Get();

                using (SqlConnection sqlConnection = new SqlConnection(
                                    GIxUtils.DecodeConnectionString(
                                        _configuration,
                                        ref _loginRequest,
                                        Request.Headers["X-WebGI-Authentication"],
                                        Request.Headers["X-WebGI-Version"])))
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
                        sqlCommand.Parameters.AddWithValue("@Username", req.username);
                        //sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        //sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

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
            try
            {
                string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
                string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();
                //string passwordEncr = GIxUtils.EncryptString(req.password);

                using (SqlConnection sqlConnection = new SqlConnection(
                                                    GIxUtils.DecodeConnectionString(
                                                        _configuration,
                                                        ref _loginRequest,
                                                        Request.Headers["X-WebGI-Authentication"],
                                                        Request.Headers["X-WebGI-Version"])))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "dbo.[usp_WebGI_IssueSessionToken]";
                        sqlCommand.Parameters.AddWithValue("@APIKey", req.apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@Username", req.username);
                        sqlCommand.Parameters.AddWithValue("@Password", req.password);

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            if (recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("IsOk")]) != System.DBNull.Value) resp.success = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("UniqueID")]) != System.DBNull.Value) req.salt = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("UserWho")]) != System.DBNull.Value) resp.userWho = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Email")]) != System.DBNull.Value) resp.email = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Avatar")]) != System.DBNull.Value) resp.avatar = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Version")]) != System.DBNull.Value) resp.version = (string)value;
                                resp.user = _loginRequest.username;
                                req.version = resp.version;
                            }
                            recordSet.Close();
                            recordSet.Dispose();
                        }
                    }

                    /////
                    /// JWT Base64 user credentials as sessionvarialbelHas + guid from DB
                    
                    var token = new JwtBuilder()
                      .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                      .WithSecret(GIxUtils.DecyptString( _configuration["JWTSecretEncypted"]))
                      .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()) // 
                      .AddClaim("LoginRequest", req)
                      .Encode();

                    //Console.WriteLine(token);
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
                throw new Exception("Invalid username or password");
            return resp.success;
        }


        [HttpPost]
        public async Task<JsonResult> Login()
        {
            LoginRequestJson reqObj = new LoginRequestJson();
            LoginResponseJson respObj = new LoginResponseJson()
            {
                success = false,
                message = "",
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
                        message = "Authorization header is not provided",
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
                        message = "Missing bearer token",
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

                    if (jbody == null)
                    {
                        object respojseObj_MissingBearer = new
                        {
                            success = false,
                            message = "Missing request body",
                            code = -401,
                            token = ""
                        };
                        return new JsonResult(respojseObj_MissingBearer);
                    }

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
                        message = "No credentials provided",
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
                GIxUtils.Log(ex);
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                object respojseObj_CheckBearer = new
                {
                    success = false,
                    message = $"Unauthenticated - {ex.Message}",
                    code = -401,
                    token = ""
                };
                return new JsonResult(respojseObj_CheckBearer);
            }


            return new JsonResult(respObj);
        }
    }
}
