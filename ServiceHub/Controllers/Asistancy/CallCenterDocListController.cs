﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using JWT.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceHub.Model;

namespace ServiceHub.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CallCenterDocListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<CallCenterDocListController> _logger;
        private readonly IConfiguration _configuration;

        public CallCenterDocListController(ILogger<CallCenterDocListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private dynamic dbGetList(ref int totalRecordCount )
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            bool exportGrid = Request.Query["type"].ToString() == "exportGrid" ? true : false;
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_CallCenterGetDocList]";
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
                        sqlCommand.Parameters.AddWithValue("@sort", Request.Query["sort"].ToString());

                        //sqlCommand.Parameters.AddWithValue("@saxeobaId", Request.Query["saxeobaId"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@docId", Request.Query["docId"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@policyNo", Request.Query["policyNo"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@pin", Request.Query["pin"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@policyHolder", Request.Query["policyHolder"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@policyHolderParent", Request.Query["policyHolderParent"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@organizationName", Request.Query["organizationName"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@contractNom", Request.Query["contractNom"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@docCreateDateStart", Request.Query["docCreateDateStart"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@docCreateDateStart", Request.Query["docCreateDateStart"].ToString());
                        //sqlCommand.Parameters.AddWithValue("@operatorId", Request.Query["operatorId"].ToString());

                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            while (recordSet.Read())
                            {
                                dynamic model = null;
                                GIGridColumn model_c = new GIGridColumn();
                                CallCenterDocListModel model_r = new CallCenterDocListModel();
                                if (initGrid == true)
                                    model = model_c;
                                else
                                    model = model_r;

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
                GIxUtils.Log(ex);
                throw new Exception(ex.Message);
            }

            if (initGrid == false)
                return rows;
            return giGridInitModel;
        }
        private bool dbDelete(ref int totalRecordCount)
        {
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();
            string signatureId = Request.Form["SignatureId"];


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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_DeleteSignature]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        sqlCommand.Parameters.AddWithValue("@SignatureId", signatureId);

                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        sqlCommand.ExecuteNonQuery();
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

            return true;
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
                rows = dbGetList(ref totalRows);
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

            return new JsonResult(new
            {
                success = rezult,
                message = exception,
                code = 0,
                total = totalRows,
                data = rows
            });
            
        }
        [HttpDelete]
        public JsonResult Delete()
        {
            int totalRows = 0;
            string exception = "Ok";
            bool rezult = true;
            object rows = new { };

            try
            {
                rows = dbGetList(ref totalRows);
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
                total = totalRows,
                data = rows
            });
        }
    }
}
