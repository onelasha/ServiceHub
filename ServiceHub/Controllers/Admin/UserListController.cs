﻿using System;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceHub.Controllers
{
    public class GIGridColumn
    {
        public string Title { get; set; }

        public string DataIndex { get; set; }
        public string DisplayField { get; set; }
        public string ValueField { get; set; }
        public string Width { get; set; }
        public string Flex { get; set; }
        public string ValueType { get; set; }

        public string Renderer { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFilter { get; set; }
        public bool IsNotColumn { get; set; }
        public bool IsHidden { get; set; }
        public bool IsMenuDisabled { get; set; }
        public bool IsGridSummaryRow { get; set; }
        public string SummaryRenderer { get; set; }
    }
    public class GIGridModel
    {
        public int RowNum { get; set; }
        public int UserId { get; set; }
		public string UserDescription { get; set; }
		public string UserCode { get; set; }
		public string Hostname { get; set; }
		public string LastLogginDate { get; set; }
        public bool IsMed { get; set; }
        public bool IsSales { get; set; }
        public bool IsBlocked { get; set; }
        public int clrfg { get; set; }
    }
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


        private void DecodeLoginRequest(IConfiguration configuration)
        {
            string secret = configuration["JWTSecret"];
            string token = Request.Headers["X-WebGI-Authentication"];

            var json = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode(token);

            dynamic jbody = JsonConvert.DeserializeObject(json);
            foreach (dynamic root in jbody)
            {
                foreach (dynamic cols in root)
                {
                    foreach (dynamic item in cols)
                    {
                        string name = item.Name;
                        string val = item.Value.ToString();
                        switch (name)
                        {
                            case "apiKey":
                                _loginRequest.apiKey = val;
                                break;
                            case "username":
                                _loginRequest.username = val;
                                break;
                            case "password":
                                _loginRequest.password = val;
                                break;
                            case "salt":
                                _loginRequest.salt = val;
                                break;
                        }
                    }
                }
            }

        }
        private string DecodeConnectionString()
        {
            string dbServer = _configuration["DBServer"];
            string dbName = _configuration["DBName"];
            DecodeLoginRequest(_configuration);
            return  $"Data Source={dbServer};Initial Catalog={dbName};Persist Security Info=True;User ID={_loginRequest.username};Password={_loginRequest.password};TrustServerCertificate=true;";
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
                using (SqlConnection sqlConnection = new SqlConnection(DecodeConnectionString()))
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
                                else {
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
            bool rezult = true;
            object rows = new { };

            try
            {
               
                rows = dbGetUserList(ref totalRows);
            }
            catch (TokenExpiredException)
            {
                rezult = false;
                Console.WriteLine("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                rezult = false;
                Console.WriteLine("Token has invalid signature");
            }
            catch (Exception ex)
            {
                rezult = false;
                Console.WriteLine(ex.Message);
            }

            return new JsonResult(new
            {
                success = rezult,
                code = 0,
                total = totalRows,
                data = rows
            });
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
