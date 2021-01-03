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
    [Route("[controller]")]
    public class DashboardListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<DashboardListController> _logger;
        private readonly IConfiguration _configuration;

        public DashboardListController(ILogger<DashboardListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }


        private dynamic dbList(ref int totalRecordCount )
        {
            //bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            //bool exportGrid = Request.Query["type"].ToString() == "exportGrid" ? true : false;
            //bool isUtil = Request.Query["subtype"].ToString() == "isUtil" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string src = Request.Query["src"].ToString();
            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();

            DashboardModel dashboardModel = new DashboardModel() {
                SaxeobaList = new List<SaxeobaModel>(),
                PieDataList = new List<PieDataModel>(),
                ActivityList = new List<ActivityModel> (),
                EntityList = new List<EntityModel>(),
                PieColorList = new List<PieColorModel>()
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetDashboard]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@Src", src);
                        //sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        //sqlCommand.Parameters.AddWithValue("@ExportGrid", exportGrid);

                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);


                        sqlCommand.Parameters.AddWithValue("@page", page);
                        sqlCommand.Parameters.AddWithValue("@start", start);
                        sqlCommand.Parameters.AddWithValue("@limit", limit);

                        //sqlCommand.Parameters.AddWithValue("@utilityFilter", Request.Query["query"].ToString()); // when typing in dropdown
                        //sqlCommand.Parameters.AddWithValue("@sort", Request.Query["sort"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@userDescription", Request.Query["userDescription"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@userCode", Request.Query["userCode"].ToString());


                        //SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        //outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            // 1. SaxeobaList
                            while (recordSet.Read())
                            {
                                dynamic model = new SaxeobaModel();
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

                                dashboardModel.SaxeobaList.Add(model);
                            }

                            // 2. PieChartData
                            recordSet.NextResult();
                            while (recordSet.Read())
                            {
                                dynamic model = new PieDataModel();
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

                                dashboardModel.PieDataList.Add(model);
                            }

                            // 3. ActivityData
                            recordSet.NextResult();
                            while (recordSet.Read())
                            {
                                dynamic model = new ActivityModel();
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
                                dashboardModel.ActivityList.Add(model);
                            }

                            // 4. Entity
                            recordSet.NextResult();
                            while (recordSet.Read())
                            {
                                dynamic model = new EntityModel();
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
                                dashboardModel.EntityList.Add(model);
                            }

                            // 4. Entity
                            recordSet.NextResult();
                            while (recordSet.Read())
                            {
                                dynamic model = new PieColorModel();
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
                                dashboardModel.PieColorList.Add(model);
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
                GIxUtils.Log(ex);
                throw new Exception(ex.Message);
            }

            return dashboardModel;
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
        }
    }
}
