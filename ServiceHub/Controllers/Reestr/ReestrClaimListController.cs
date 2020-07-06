using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
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
    public class ReestrClaimListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<ReestrClaimListController> _logger;
        private readonly IConfiguration _configuration;

        public ReestrClaimListController(ILogger<ReestrClaimListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        //private IEnumerable<dynamic> dbGetProviderList(ref int totalRecordCount )
        private dynamic dbGetClaimsList(ref int totalRecordCount)
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            bool exportGrid = Request.Query["type"].ToString() == "exportGrid" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            List<dynamic> rows = new List<dynamic>();
            GIGridInitModel giGridInitModel = new GIGridInitModel()
            {
                ColumnList = new List<GIGridColumn>()
            };
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetClaimsList]";
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);

                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        if (Request != null && Request.Query != null && Request.Query.Keys != null && Request.Query.Keys.Count > 0)
                        {
                            foreach (string key in Request.Query.Keys)
                            {
                                if (!key.StartsWith("_") )
                                {
                                    string param = $"@{key}";
                                    sqlCommand.Parameters.AddWithValue(param, Request.Query[key].ToString());
                                }
                            }
                        };

                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            while (recordSet.Read())
                            {
                                dynamic model = null;
                                if (initGrid == true)
                                    model = new GIGridColumn();
                                else
                                    model = new ContractListModel(); ///////////////// !!!!!!!! //////////

                                var properties = model.GetType().GetProperties();
                                foreach (var el in properties)
                                {
                                    string name = el.Name;
                                    value = recordSet[recordSet.GetOrdinal(name)];
                                    if (value != System.DBNull.Value)
                                    {
                                        switch (el.PropertyType.Name)
                                        {
                                            case "Int32":
                                                el.SetValue(model, (int)value);
                                                break;
                                            case "String":
                                                el.SetValue(model, (string)value);
                                                break;
                                            case "Boolean":
                                                el.SetValue(model, (bool)value);
                                                break;
                                            case "Decimal":
                                                el.SetValue(model, (decimal)value);
                                                break;
                                            case "DateTime":
                                                el.SetValue(model, (DateTime)value);
                                                break;
                                        }

                                    }
                                }

                                if (initGrid == true) 
                                    giGridInitModel.ColumnList.Add(model);
                                else 
                                    rows.Add(model);
                            }

                            if (initGrid == true && recordSet.NextResult() && recordSet.Read())
                            {
                                if ((value = recordSet[recordSet.GetOrdinal("Title")]) != System.DBNull.Value) giGridInitModel.Title = (string)value;
                                if ((value = recordSet[recordSet.GetOrdinal("Toolbar")]) != System.DBNull.Value) giGridInitModel.Toolbar = (string)value;
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

            if (initGrid == false)
                return rows;
            return giGridInitModel;
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

                rows = dbGetClaimsList(ref totalRows);
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
                rows = new
                {
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
