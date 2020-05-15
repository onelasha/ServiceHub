using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceHub.Model;

namespace ServiceHub.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<UserListController> _logger;
        private readonly IConfiguration _configuration;

        public UserListController(ILogger<UserListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private IEnumerable<dynamic> dbGetUserList(ref int totalRecordCount )
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();



            List<dynamic> rows = new List<dynamic>();
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetUserList]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);


                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        sqlCommand.Parameters.AddWithValue("@page", page);
                        sqlCommand.Parameters.AddWithValue("@start", start);
                        sqlCommand.Parameters.AddWithValue("@limit", limit);

                        sqlCommand.Parameters.AddWithValue("@sort", Request.Query["sort"].ToString());
                        sqlCommand.Parameters.AddWithValue("@userDescription", Request.Query["userDescription"].ToString());
                        sqlCommand.Parameters.AddWithValue("@userCode", Request.Query["userCode"].ToString());


                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            while (recordSet.Read())
                            {

                                if (initGrid == true)
                                {
                                    GIGridColumn column = new GIGridColumn();
                                    if ((value = recordSet[recordSet.GetOrdinal("Title")]) != System.DBNull.Value) column.Title = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("DataIndex")]) != System.DBNull.Value) column.DataIndex = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("DisplayField")]) != System.DBNull.Value) column.DisplayField = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("ValueField")]) != System.DBNull.Value) column.ValueField = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("ValueType")]) != System.DBNull.Value) column.ValueType = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("Width")]) != System.DBNull.Value) column.Width = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("Flex")]) != System.DBNull.Value) column.Flex = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("Renderer")]) != System.DBNull.Value) column.Renderer = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsFilter")]) != System.DBNull.Value) column.IsFilter = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsNotColumn")]) != System.DBNull.Value) column.IsNotColumn = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsHidden")]) != System.DBNull.Value) column.IsHidden = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsMenuDisabled")]) != System.DBNull.Value) column.IsMenuDisabled = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsGridSummaryRow")]) != System.DBNull.Value) column.IsGridSummaryRow = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsLocked")]) != System.DBNull.Value) column.IsLocked = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("SummaryRenderer")]) != System.DBNull.Value) column.SummaryRenderer = (string)value;

                                    rows.Add(column);
                                }
                                else
                                {
                                    GIGridModel model = new GIGridModel();
                                    if ((value = recordSet[recordSet.GetOrdinal("RowNum")]) != System.DBNull.Value) model.RowNum = (int)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("UserId")]) != System.DBNull.Value) model.UserId = (int)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("UserDescription")]) != System.DBNull.Value) model.UserDescription = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("UserCode")]) != System.DBNull.Value) model.UserCode = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("Hostname")]) != System.DBNull.Value) model.Hostname = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("LastLogginDate")]) != System.DBNull.Value) model.LastLogginDate = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsMed")]) != System.DBNull.Value) model.IsMed = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsBlocked")]) != System.DBNull.Value) model.IsBlocked = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("IsSales")]) != System.DBNull.Value) model.IsSales = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("clrfg")]) != System.DBNull.Value) model.clrfg = (int)value;

                                    rows.Add(model);
                                }
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
                throw new Exception(ex.Message);
            }

            return rows;
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
               
                rows = dbGetUserList(ref totalRows);
            }
            catch (TokenExpiredException ex)
            {
                rezult = false;
                exception = ex.Message;
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException ex)
            {
                rezult = false;
                exception = ex.Message;
                Console.WriteLine("Token has invalid signature");
            }
            catch (Exception ex)
            {
                rezult = false;
                exception = ex.Message;
                Console.WriteLine(ex.Message);
                rows = new {
                    message = exception
                };
            }

            return new JsonResult(new
            {
                success = rezult,
                message = exception,
                code = 0,
                total = totalRows,
                data = rows
            });
            
        }
    }
}
