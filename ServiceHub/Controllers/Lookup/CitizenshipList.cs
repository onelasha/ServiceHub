using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using ServiceHub.Model;

namespace ServiceHub.Controllers
{
    [ApiController]
    [Route("lookup/[controller]")]
    public class CitizenshipListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<CitizenshipListController> _logger;
        private readonly IConfiguration _configuration;

        public CitizenshipListController(ILogger<CitizenshipListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }


        private dynamic dbList(ref int totalRecordCount )
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            bool exportGrid = Request.Query["type"].ToString() == "exportGrid" ? true : false;
            bool isUtil = Request.Query["subtype"].ToString() == "isUtil" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();



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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_lookup_GetCitizenshipList]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        sqlCommand.Parameters.AddWithValue("@ExportGrid", exportGrid);

                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);


                        sqlCommand.Parameters.AddWithValue("@page", page);
                        sqlCommand.Parameters.AddWithValue("@start", start);
                        sqlCommand.Parameters.AddWithValue("@limit", limit);

                        sqlCommand.Parameters.AddWithValue("@utilityFilter", Request.Query["utilityFilter"].ToString()); // when typing in dropdown
                        sqlCommand.Parameters.AddWithValue("@sort", Request.Query["sort"].ToString());

                        //sqlCommand.Parameters.AddWithValue("@userDescription", Request.Query["userDescription"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@userCode", Request.Query["userCode"].ToString());


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

                                    giGridInitModel.ColumnList.Add(column);
                                }
                                else
                                {
                                    GILookupModel model = new GILookupModel();
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
                                            }

                                        }
                                    }
                                    rows.Add(model);
                                }
                                //else {
                                //    GILookupModel model = new GILookupModel();
                                //    if ((value = recordSet[recordSet.GetOrdinal("Id")]) != System.DBNull.Value) model.Id = (int)value;
                                //    if ((value = recordSet[recordSet.GetOrdinal("Description")]) != System.DBNull.Value) model.Description = (string)value;
                                //    rows.Add(model);
                                //}
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
                GIxUtils.Log(ex);
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
            bool rezult = true;
            string rezultMessage = string.Empty;
            object rows = new { };

            try
            {
               
                rows = dbList(ref totalRows);
            }
            catch (TokenExpiredException ex)
            {
                rezult = false;
                rezultMessage = "Token has expired";
                GIxUtils.Log(ex);
            }
            catch (SignatureVerificationException ex)
            {
                rezult = false;
                rezultMessage = "Token has invalid signature";
                GIxUtils.Log(ex);
            }
            catch (Exception ex)
            {
                rezult = false;
                rezultMessage = ex.Message;
                GIxUtils.Log(ex);
            }

            return new JsonResult(new
            {
                success = rezult,
                code = 0,
                message = rezultMessage,
                total = totalRows,
                data = rows
            }); ;
            //var rng = new Random();
            //return new JsonResult( new { success = true, status = "success",  Data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureCC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //}) });

            /*
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureCC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            */
        }
    }
}
