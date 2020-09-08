using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceHub.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ServiceHub.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private dynamic dbGetUserUser(ref int totalRecordCount )
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();
            string userId = Request.Query["UserId"];


            UserModel model = new UserModel();

            //List<dynamic> rows = new List<dynamic>();
            try
            {
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetUser]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        sqlCommand.Parameters.AddWithValue("@UserId", userId);
                        

                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            if (recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("UserId")]) != System.DBNull.Value) model.UserId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StaffId")]) != System.DBNull.Value) model.StaffId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("FirstName")]) != System.DBNull.Value) model.FirstName = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("LastName")]) != System.DBNull.Value) model.LastName = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DOB")]) != System.DBNull.Value) model.DOB = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Address")]) != System.DBNull.Value) model.Address = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PassportNom")]) != System.DBNull.Value) model.PassportNom = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PIN")]) != System.DBNull.Value) model.PIN = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("CitizenshipId")]) != System.DBNull.Value) model.CitizenshipId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Citizenship")]) != System.DBNull.Value) model.Citizenship = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("ContractNom")]) != System.DBNull.Value) model.ContractNom = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DateStart")]) != System.DBNull.Value) model.DateStart = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DateEnd")]) != System.DBNull.Value) model.DateEnd = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StatusId")]) != System.DBNull.Value) model.StatusId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StatusDescription")]) != System.DBNull.Value) model.StatusDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DepartmentId")]) != System.DBNull.Value) model.DepartmentId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DepartmentDescription")]) != System.DBNull.Value) model.DepartmentDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PositionId")]) != System.DBNull.Value) model.PositionId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PositionDescription")]) != System.DBNull.Value) model.PositionDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Code")]) != System.DBNull.Value) model.Code = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Username")]) != System.DBNull.Value) model.Username = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsBlocked")]) != System.DBNull.Value) model.IsBlocked = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsMed")]) != System.DBNull.Value) model.IsMed = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsSales")]) != System.DBNull.Value) model.IsSales = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("BirthPlace")]) != System.DBNull.Value) model.BirthPlace = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Email")]) != System.DBNull.Value) model.Email = (string)value;


                            }
                            recordSet.Close();
                            recordSet.Dispose();

                            if (outputValue.Value != null)
                                totalRecordCount = (int)outputValue.Value;
                        }
                    }

                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }

            catch (Exception ex)
            {
                GIxUtils.Log(ex);
                throw new Exception(ex.Message);
            }

            return model;
        }
        private dynamic dbSetUserUser(UserModel user, ref int totalRecordCount)
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();
            string userId = Request.Query["UserId"];


            UserModel model = new UserModel();
            try
            {
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_SetUser]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        sqlCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        sqlCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        sqlCommand.Parameters.AddWithValue("@DOB", user.DOB);
                        sqlCommand.Parameters.AddWithValue("@Address", user.Address);
                        sqlCommand.Parameters.AddWithValue("@BirthPlace", user.BirthPlace);
                        sqlCommand.Parameters.AddWithValue("@PIN", user.PIN);
                        sqlCommand.Parameters.AddWithValue("@PassportNom", user.PassportNom);
                        sqlCommand.Parameters.AddWithValue("@CitizenshipId", user.CitizenshipId);
                        sqlCommand.Parameters.AddWithValue("@ContractNom", user.ContractNom);
                        sqlCommand.Parameters.AddWithValue("@DateStart", user.DateStart);
                        sqlCommand.Parameters.AddWithValue("@DateEnd", user.DateEnd);
                        sqlCommand.Parameters.AddWithValue("@StatusId", user.StatusId);
                        sqlCommand.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
                        sqlCommand.Parameters.AddWithValue("@PositionId", user.PositionId);
                        sqlCommand.Parameters.AddWithValue("@UserId", user.UserId);
                        sqlCommand.Parameters.AddWithValue("@StaffId", user.StaffId);
                        sqlCommand.Parameters.AddWithValue("@IsMed", user.IsMed == null ? false : Convert.ToBoolean(user.IsMed.ToString()));
                        sqlCommand.Parameters.AddWithValue("@IsBlocked", user.IsBlocked == null ? false : Convert.ToBoolean(user.IsBlocked.ToString()));
                        sqlCommand.Parameters.AddWithValue("@IsSales", user.IsSales == null ? false : Convert.ToBoolean(user.IsSales.ToString()));
                        sqlCommand.Parameters.AddWithValue("@Code", user.Code);
                        sqlCommand.Parameters.AddWithValue("@Permissions", user.Permissions);
                        sqlCommand.Parameters.AddWithValue("@Email", user.Email);

                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            if (recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("UserId")]) != System.DBNull.Value) model.UserId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StaffId")]) != System.DBNull.Value) model.StaffId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("FirstName")]) != System.DBNull.Value) model.FirstName = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("LastName")]) != System.DBNull.Value) model.LastName = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DOB")]) != System.DBNull.Value) model.DOB = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Address")]) != System.DBNull.Value) model.Address = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PIN")]) != System.DBNull.Value) model.PIN = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PassportNom")]) != System.DBNull.Value) model.PassportNom = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("CitizenshipId")]) != System.DBNull.Value) model.CitizenshipId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Citizenship")]) != System.DBNull.Value) model.Citizenship = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("ContractNom")]) != System.DBNull.Value) model.ContractNom = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DateStart")]) != System.DBNull.Value) model.DateStart = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DateEnd")]) != System.DBNull.Value) model.DateEnd = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StatusId")]) != System.DBNull.Value) model.StatusId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("StatusDescription")]) != System.DBNull.Value) model.StatusDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DepartmentId")]) != System.DBNull.Value) model.DepartmentId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("DepartmentDescription")]) != System.DBNull.Value) model.DepartmentDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PositionId")]) != System.DBNull.Value) model.PositionId = (int)value;
                                if ((value = recordSet[recordSet.GetOrdinal("PositionDescription")]) != System.DBNull.Value) model.PositionDescription = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Code")]) != System.DBNull.Value) model.Code = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Username")]) != System.DBNull.Value) model.Username = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsBlocked")]) != System.DBNull.Value) model.IsBlocked = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsMed")]) != System.DBNull.Value) model.IsMed = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("IsSales")]) != System.DBNull.Value) model.IsSales = (bool)value;
                                if ((value = recordSet[recordSet.GetOrdinal("BirthPlace")]) != System.DBNull.Value) model.BirthPlace = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Email")]) != System.DBNull.Value) model.Email = (string)value;
                            }
                            recordSet.Close();
                            recordSet.Dispose();

                            if (outputValue.Value != null)
                                totalRecordCount = (int)outputValue.Value;
                        }
                    }

                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }

            catch (Exception ex)
            {
                GIxUtils.Log(ex);
                throw new Exception(ex.Message);
            }

            return model;
        }


        [HttpGet]
        public JsonResult Get()
        {

            int totalRows = 0;
            string exception = "Ok";
            bool rezult = true;
            object rows = new { };

            try
            {
                rows = dbGetUserUser(ref totalRows);
            }
            catch (TokenExpiredException ex)
            {
                rezult = false;
                exception = ex.Message;
                GIxUtils.Log(ex);
            }
            catch (SignatureVerificationException ex)
            {
                rezult = false;
                exception = ex.Message;
                GIxUtils.Log(ex);
            }
            catch (Exception ex)
            {
                rezult = false;
                exception = ex.Message;
                Console.WriteLine(ex.Message);
                rows = new {
                    message = exception
                };
                GIxUtils.Log(ex);
            }

            //return new JsonResult(new { success= rezult, message = exception, records = 1, root = ".", children = rows });
            return new JsonResult(new
            {
                success = rezult,
                message = exception,
                code = 0,
                total = totalRows,
                data = rows
            });
        }

        [HttpPost]
        public JsonResult Post([FromBody] object content) 
        {
            int totalRows = 0;
            string exception = "Ok";
            bool rezult = true;
            object rows = new { };

            //UserModel user = new UserModel();

            try
            {
                string vs = content.ToString();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                UserModel user = JsonSerializer.Deserialize<UserModel>(vs, options);
                rows = dbSetUserUser(user, ref totalRows);
            }
            catch (TokenExpiredException ex)
            {
                rezult = false;
                exception = ex.Message;
                GIxUtils.Log(ex);
            }
            catch (SignatureVerificationException ex)
            {
                rezult = false;
                exception = ex.Message;
                GIxUtils.Log(ex);
            }
            catch (Exception ex)
            {
                rezult = false;
                exception = ex.Message;
                Console.WriteLine(ex.Message);
                rows = new
                {
                    message = exception
                };
                GIxUtils.Log(ex);
            }

            return new JsonResult(new
            {
                success = rezult,
                message = exception,
                code = 0,
                total = 0,
                data = rows
            });
        }
    }
}
