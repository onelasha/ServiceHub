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
    public class LeftMenuListController : ControllerBase
    {

        private LoginRequestJson _loginRequest;
        private readonly ILogger<LeftMenuListController> _logger;
        private readonly IConfiguration _configuration;

        public LeftMenuListController(ILogger<LeftMenuListController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _loginRequest = new LoginRequestJson();
        }

        private IEnumerable<dynamic> dbGetUserLeftMenuList(ref int totalRecordCount )
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
                        sqlCommand.CommandText = "dbo.[usp_WebGI_GetUserLeftMenuList]";
                        //sqlCommand.Parameters.AddWithValue("@APIKey", apiKey);
                        sqlCommand.Parameters.AddWithValue("@IP_Local", localIP);
                        sqlCommand.Parameters.AddWithValue("@IP_Remote", remoteIP);
                        sqlCommand.Parameters.AddWithValue("@InitGrid", initGrid);
                        sqlCommand.Parameters.AddWithValue("@Salt", _loginRequest.salt);
                        sqlCommand.Parameters.AddWithValue("@Version", _loginRequest.version);

                        //sqlCommand.Parameters.AddWithValue("@page", page);
                        //sqlCommand.Parameters.AddWithValue("@start", start);
                        //sqlCommand.Parameters.AddWithValue("@limit", limit);

                        //sqlCommand.Parameters.AddWithValue("@sort", Request.Query["sort"].ToString());
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

                                //if (initGrid == true)
                                //{
                                //    GIGridColumn column = new GIGridColumn();
                                //    if ((value = recordSet[recordSet.GetOrdinal("Title")]) != System.DBNull.Value) column.Title = (string)value;
                                //    rows.Add(column);
                                //}
                                //else
                                {
                                    LeftMenu model = new LeftMenu();
                                    if ((value = recordSet[recordSet.GetOrdinal("rootId")]) != System.DBNull.Value) model.rootId = (int)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("id")]) != System.DBNull.Value) model.id = (int)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("pid")]) != System.DBNull.Value) model.pid = (int)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("leaf")]) != System.DBNull.Value) model.leaf = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("expanded")]) != System.DBNull.Value) model.expanded = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("loaded")]) != System.DBNull.Value) model.loaded = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("text")]) != System.DBNull.Value) model.text = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("iconCls")]) != System.DBNull.Value) model.iconCls = (string)value;

                                    if ((value = recordSet[recordSet.GetOrdinal("isMenuGroup")]) != System.DBNull.Value) model.isMenuGroup = (bool)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("reference")]) != System.DBNull.Value) model.reference = (string)value;
                                    if ((value = recordSet[recordSet.GetOrdinal("url")]) != System.DBNull.Value) model.url = (string)value;

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
                GIxUtils.Log(ex);
                throw new Exception(ex.Message);
            }

            return rows;
        }

        private IEnumerable<dynamic> MakePlanTree(object list)
        {
            IEnumerable<dynamic> rows = (IEnumerable<dynamic>)list;

            var _list = from i in rows
                        where i.isMenuGroup == true
                        select new LeftMenu()
                        {
                            rootId = i.rootId,
                            id = i.id,
                            Checked = true,
                            pid = i.pid,
                            loaded = i.loaded,
                            expanded = i.expanded,
                            leaf = i.leaf,
                            iconCls = i.iconCls,
                            text = i.text,
                            url = i.url,
                            reference = i.reference,
                            children = (from j in rows
                                        where j.pid == i.id
                                            && j.isMenuGroup == false
                                        select new LeftMenu()
                                        {
                                            id = j.id,
                                            Checked = true,
                                            pid = j.pid,
                                            rootId = j.rootId,
                                            loaded = j.loaded,
                                            expanded = j.expanded,
                                            leaf = j.leaf,
                                            iconCls = j.iconCls,
                                            text = j.text,
                                            url = j.url,
                                            reference = j.reference,
                                            children = null
                                        })
                        };


            return _list;
/*
            var _list = from i in list
                        group i by new { i.PolicyYear } into grp
                        orderby grp.Key.PolicyYear descending
                        select new PlanTree()
                        {
                            text = grp.Key.PolicyYear,
                            year = grp.Key.PolicyYear,
                            loaded = true,
                            leaf = false,
                            iconCls = "iconPolicyYear",
                            children = (from j in list
                                        where j.PolicyYear == grp.Key.PolicyYear
                                        group j by new { j.SchoolId, j.Type, j.PolicyYear } into sg
                                        orderby sg.Key.Type
                                        select new PlanTree()
                                        {
                                            year = sg.Key.PolicyYear,
                                            text = sg.Key.Type,
                                            loaded = true,
                                            leaf = false,
                                            schoolId = sg.Key.SchoolId,
                                            iconCls = "iconSchool",
                                            children = (from k in list
                                                        where k.PolicyYear == grp.Key.PolicyYear
                                                           && k.SchoolId == sg.Key.SchoolId
                                                        orderby k.GroupName
                                                        select new PlanTree()
                                                        {
                                                            year = k.PolicyYear,
                                                            text = k.GroupCode + " " + k.GroupName,
                                                            schoolId = k.SchoolId,
                                                            rootId = k.PolicyId,
                                                            hasTagId = k.HasTagId,
                                                            loaded = true,
                                                            leaf = true,
                                                            iconCls = "iconGroup"
                                                        }).ToList()
                                        }).ToList()
                        };

            return _list.ToList();
*/
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

                //dynamic car = new Object();
                //car.AddProperty("TopSpeed", 180);


                rows = dbGetUserLeftMenuList(ref totalRows);
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
                rows = new {
                    message = exception
                };
                GIxUtils.Log(ex);
            }

            return new JsonResult(new { success= rezult, message = exception, records = 1, root = ".", children = rows });

            //return new JsonResult(new
            //{
            //    success = rezult,
            //    message = exception,
            //    code = 0,
            //    total = totalRows,
            //    data = rows
            //});

        }
    }
}
