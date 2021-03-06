﻿using System;
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
using JWT.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceHub.Model;

namespace ServiceHub.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TaskPermissionListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<TaskPermissionListController> _logger;
        private readonly IConfiguration _configuration;

        public TaskPermissionListController(ILogger<TaskPermissionListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private IEnumerable<dynamic> dbGetUserTaskPermissionList(ref int totalRecordCount)
        {
            bool initGrid = Request.Query["type"].ToString() == "initGrid" ? true : false;
            string remoteIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
            string localIP = this.HttpContext.Connection.LocalIpAddress.ToString();

            string page = Request.Query["page"].ToString();
            string start = Request.Query["start"].ToString();
            string limit = Request.Query["limit"].ToString();
            string taskId = Request.Query["TaskId"];



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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetTaskPermisionList]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        sqlCommand.Parameters.AddWithValue("@TaskId", taskId);


                        SqlParameter outputValue = sqlCommand.Parameters.Add("@totalCount", SqlDbType.Int);
                        outputValue.Direction = ParameterDirection.Output;

                        SqlDataReader recordSet = sqlCommand.ExecuteReader();
                        using (recordSet)
                        {
                            object value;
                            while (recordSet.Read())
                            {
                                UserPermissionModel model = new UserPermissionModel();
                                //if ((value = recordSet[recordSet.GetOrdinal("rootId")]) != System.DBNull.Value) model.rootId = (int)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("id")]) != System.DBNull.Value) model.id = (int)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("pid")]) != System.DBNull.Value) model.pid = (int)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("leaf")]) != System.DBNull.Value) model.leaf = (bool)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("hasAccess")]) != System.DBNull.Value) model.hasAccess = (bool)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("expanded")]) != System.DBNull.Value) model.expanded = (bool)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("loaded")]) != System.DBNull.Value) model.loaded = (bool)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("text")]) != System.DBNull.Value) model.text = (string)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("cls")]) != System.DBNull.Value) model.cls = (string)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("iconCls")]) != System.DBNull.Value) model.iconCls = (string)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("reference")]) != System.DBNull.Value) model.reference = (string)value;
                                //if ((value = recordSet[recordSet.GetOrdinal("url")]) != System.DBNull.Value) model.url = (string)value;

                                //rows.Add(model);
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

            return rows;
        }
        private IEnumerable<dynamic> GetChildren(object list, int rootId = 0)
        {
            IEnumerable<dynamic> rows = (IEnumerable<dynamic>)list;
            IEnumerable<dynamic> subrows = from i in rows
                        where rootId == i.pid && i.rootId != rootId
                        group i by i.pid into children
                        select new
                        {
                            Count = children.Count()
                        };

            if (subrows.Count() == 0)
                return null;

            var _list = from i in rows
                        where rootId == i.pid && i.rootId != rootId
                        select new UserPermissionModel()
                        {
                            rootId = i.rootId,
                            id = i.id,
                            hasAccess = i.hasAccess,
                            Checked = i.hasAccess,
                            pid = i.pid,
                            loaded = i.loaded,
                            expanded = i.expanded,
                            leaf = i.leaf,
                            iconCls = i.iconCls,
                            text = i.text,
                            url = i.url,
                            cls = i.cls,
                            reference = i.reference,
                            children = GetChildren(list, i.rootId)
                        };
            return _list;
        }
        private IEnumerable<dynamic> MakePlanTree(object list )
        {
            IEnumerable<dynamic> rows = (IEnumerable<dynamic>)list;

            var _list = from i in rows
                        where i.rootId == i.pid
                        select new UserPermissionModel()
                        {
                            rootId = i.rootId,
                            id = i.id,
                            hasAccess = i.hasAccess,
                            Checked = i.hasAccess,
                            pid = i.pid,
                            loaded = i.loaded,
                            expanded = i.expanded,
                            leaf = i.leaf,
                            iconCls = i.iconCls,
                            text = i.text,
                            url = i.url,
                            cls = i.cls,
                            reference = i.reference,
                            children = GetChildren(list,i.rootId)
                        };

            return _list;

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
                rows = dbGetUserTaskPermissionList(ref totalRows);
                rows = MakePlanTree(rows);
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

            return new JsonResult(new { success = rezult, message = exception, records = 1, root = ".", children = rows });
        }
    }
}
