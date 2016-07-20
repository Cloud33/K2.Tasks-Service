using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tasks.Service;
using Tasks.Service.Dto;
using TasksWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TasksWeb.Help;
using System.Dynamic;
using System.Data;

namespace TasksWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly WorkflowClientService client;
        private readonly string QuanShiCheckAPIUrl = ConfigurationManager.AppSettings["QuanShiCheckAPIUrl"];
        public HomeController()
        {
            client = new WorkflowClientService();
        }
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetTaskList(string username)
        {
            DataModel dataModel = new DataModel();
            try
            {
                //System.Threading.Thread.Sleep(2000);
                logger.Info("GetTaskList请求，请求信息：" + username);
                dataModel.status = RequestStatus.Success;
                dataModel.message = "";
                int totalCount = 0;
                if (username.Contains("@"))
                {
                    string[] arr = username.Split('@');
                    if (arr.Length > 1)
                    {
                        username = arr[0];
                    }
                    else
                    {
                        throw new Exception("uc_account为:" + username + " 出现问题，请联系管理员处理。");
                    }
                }
                var worklist = client.GetTasksItems(username, null, null, out totalCount);
                dataModel.data = worklist;
                logger.Info("GetTaskList请求成功，返回的信息：" + JsonConvert.SerializeObject(dataModel));
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在获取任务列表时程序出现异常，错误的信息：" + ex.Message +
                   System.Environment.NewLine + "错误详细信息：" + ex.StackTrace;
                dataModel.data = new List<string>();
                logger.Error("GetTaskList请求失败，在执行过程中出现异常", ex);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        private JsonResult GetTaskList2()
        {
            DataModel dataModel = new DataModel();
            try
            {
                dataModel.status = RequestStatus.Success;
                dataModel.message = "";
                int totalCount = 0;
                var worklist = client.GetWorklistItem(@"K2:DENALLIX\Bob", null, null, out totalCount);
                dataModel.data = worklist;
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在获取任务信息时报错，错误的信息：" + ex.Message;
                dataModel.data = new List<string>();
                logger.Error("Login请求失败，在执行过程中出现异常", ex);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetTaskInfo(ReceiveModel mode)
        {
            DataModel dataModel = new DataModel();
            try
            {
                //System.Threading.Thread.Sleep(2000);
                logger.Info("GetTaskInfo请求，请求信息：" + JsonConvert.SerializeObject(mode));
                dataModel.status = RequestStatus.Success;
                dataModel.message = "";
                //int totalCount = 0;
                dataModel.data = client.OpenWorklistItem(mode.Destination, mode.SN, mode.SharedUser);//_tasks.Where(t => t.Folio == id).FirstOrDefault();
                logger.Info("GetTaskInfo请求成功，返回信息：" + JsonConvert.SerializeObject(dataModel));
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在获取任务信息时程序出现异常，错误的信息：" + ex.Message +
                  System.Environment.NewLine + "错误详细信息：" + ex.StackTrace;
                dataModel.data = new List<string>();
                logger.Error("GetTaskInfo请求失败，在执行过程中出现异常", ex);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExecuteTask(ReceiveModel mode)
        {
            DataModel dataModel = new DataModel();
            try
            {
                //System.Threading.Thread.Sleep(2000);
                string data = JsonConvert.SerializeObject(mode);
                logger.Info("ExecuteTask请求，请求信息：" + data);
                client.ExecuteAction(mode.Destination, mode.SN, mode.ActionName, false, mode.SharedUser);
                dataModel.status = RequestStatus.Success;
                dataModel.message = "";
                dataModel.data = new List<string>();//_tasks.Where(t => t.Folio == id).FirstOrDefault();
                logger.Info("ExecuteTask请求成功，审批成功，请求信息：" + data);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在审批任务时程序出现异常，错误的信息：" + ex.Message +
                  System.Environment.NewLine + "错误详细信息：" + ex.StackTrace;
                dataModel.data = new List<string>();
                logger.Error("ExecuteTask请求失败，在执行过程中出现异常", ex);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult QuanShiExecuteTask(ReceiveModel mode)
        {
            OANewsChangeRequestModel startu = new OANewsChangeRequestModel();
            try
            {
                startu.status = 1;
                string data = JsonConvert.SerializeObject(mode);
                logger.Info("QuanShiExecuteTask请求，请求信息：" + data);
                if (mode.SharedUser == "null")
                {
                    mode.SharedUser = "";
                }
                client.ExecuteAction(System.Web.HttpUtility.UrlDecode(mode.Destination), mode.SN, System.Web.HttpUtility.UrlDecode(mode.ActionName), false, mode.SharedUser);
                TaskPush.QuanShiPushChange(mode.ProcInstID, startu.status, logger);
                logger.Info("QuanShiExecuteTask请求成功，审批成功，请求信息：" + data);
                return Json(startu, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("24411"))
                {
                    if (ex.Message.Contains("is not allowed to open the worklist item"))
                    {
                        startu.status = 9;
                        TaskPush.QuanShiPushChange(mode.ProcInstID, startu.status, logger);
                        logger.Error("ExecuteTask请求失败，任务已经被处理！");

                        return Json(startu, JsonRequestBehavior.AllowGet);
                    }
                }
                logger.Error("ExecuteTask请求失败，在执行过程中出现异常", ex);
                startu.status = 12;
                TaskPush.QuanShiPushChange(mode.ProcInstID, startu.status, logger);
                return Json(startu, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Login(CheckModel checkModel)
        {
            DataModel dataModel = new DataModel();
            try
            {
                string data = TaskPush.CreateQuanShiToKen();
                LoginRequestModel re = new LoginRequestModel();
                re = JsonConvert.DeserializeObject<LoginRequestModel>(data);
                if (re.crrorCode == 0)
                {
                    //checkModel.token = re.data.token;
                    string username = checkModel.username;
                    string token = checkModel.token;
                    checkModel.username = re.data.username;
                    checkModel.token = re.data.token;
                    List<CheckDataModel> list = new List<CheckDataModel>();
                    CheckDataModel item = new CheckDataModel();
                    item.account = username;
                    item.sessionId = token;
                    list.Add(item);
                    checkModel.data = list;
                    string dataCheck = TaskPush.PostWebRequest(QuanShiCheckAPIUrl, JsonConvert.SerializeObject(checkModel), Encoding.UTF8);
                    JObject obj = JObject.Parse(dataCheck);
                    if (obj["data"][0]["errorCode"].ToString() == "0")
                    {
                        string user = obj["data"][0]["account"].ToString();
                        dataModel.status = RequestStatus.Success;
                        dataModel.message = "";
                        dataModel.data = user; ;//_tasks.Where(t => t.Folio == id).FirstOrDefault();
                        logger.Info("Login请求成功，Lgoin User：" + user);
                        return Json(dataModel, JsonRequestBehavior.AllowGet);
                    }
                    dataModel.status = RequestStatus.Error;
                    dataModel.message = "全时token检查失败，errorCode：" + obj["data"][0]["errorCode"].ToString();
                    dataModel.data = new List<string>();//_tasks.Where(t => t.Folio == id).FirstOrDefault();
                    logger.Error("Login请求失败，在检查session 状态失败，请求account：" + username + ",返回值：" + dataCheck);
                    return Json(dataModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    dataModel.status = RequestStatus.Error;
                    dataModel.message = "蜜蜂OpenAPI登录失败，errorCode：" + re.crrorCode.ToString();
                    dataModel.data = new List<string>();
                    logger.Error("Login请求失败，在使用管理员登录蜜蜂Open API失败，错误：" + data +
                        System.Environment.NewLine + "错误信息：" + re.errorMessage);
                    return Json(dataModel, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在验证身份时程序出现异常，错误的信息：" + ex.Message +
                  System.Environment.NewLine + "错误详细信息：" + ex.StackTrace;
                dataModel.data = new List<string>();
                logger.Error("Login请求失败，在执行过程中出现异常", ex);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetBusinessData(int procInstID)
        {
            DataModel dataModel = new DataModel();
            try
            {
                DataTable dt = TaskPush.GetBusinessJsonData(procInstID);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dc = dt.Rows[0];
                        string folio = GetFolioHeader(dc["ProcessFolio"].ToString());
                        switch (folio)
                        {
                            case "EC":
                                dataModel.status = RequestStatus.Success;
                                dataModel.message = "";
                                dataModel.data = GetExpenseClaimData(dc);
                                break;
                            case "TR":
                                dataModel.status = RequestStatus.Success;
                                dataModel.message = "";
                                dataModel.data = GetTravelRequestData(dc);
                                break;
                            default:
                                dataModel.status = RequestStatus.Error;
                                dataModel.message = "未定义流程业务";
                                dataModel.data = new Object();
                                break;
                        }
                    }
                    else
                    {
                        dataModel.status = RequestStatus.Error;
                        dataModel.message = "没有获取到业务数据";
                        dataModel.data = new Object();
                    }
                }
                else
                {
                    dataModel.status = RequestStatus.Error;
                    dataModel.message = "没有获取到业务数据";
                    dataModel.data = new Object();
                }

            }
            catch (Exception ex)
            {
                dataModel.status = RequestStatus.Error;
                dataModel.message = "在获取业务信息时程序出现异常，错误的信息：" + ex.Message +
                  System.Environment.NewLine + "错误详细信息：" + ex.StackTrace;
                dataModel.data = new List<string>();
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }


        private Object GetExpenseClaimData(DataRow dc)
        {
            dynamic jsonObj = JObject.Parse(dc["JsonData"].ToString());
            JArray ja = (JArray)JsonConvert.DeserializeObject(jsonObj["ExpenseClaimDetailModels"].ToString());
            List<ExpenseClaimDetailsModel> list = new List<ExpenseClaimDetailsModel>();
            if (ja.Count > 0)
            {
                for (int i = 0; i < ja.Count; i++)
                {
                    ExpenseClaimDetailsModel item = new ExpenseClaimDetailsModel();
                    item.日期 = Convert.ToDateTime(ja[i]["ExpenseDate"]).ToString("yyyy-MM-dd");
                    item.类型 = ja[i]["Category"].ToString();
                    item.描述 = ja[i]["Description"].ToString();
                    item.金额 = ja[i]["Amount"].ToString();
                    list.Add(item);
                }
            }
            ExpenseClaimModel obj = new ExpenseClaimModel();
            obj.流程单号 = dc["ProcessFolio"].ToString();
            obj.报销月份 = Convert.ToDateTime(jsonObj["ExpenseMonth"]).ToString("yyyy-MM");
            obj.本位币 = jsonObj["BaseCurrency"].ToString();
            obj.项目名称 = jsonObj["ProjectReference"].ToString();
            obj.说明 = jsonObj["Notes"].ToString();
            obj.详细 = list;
            return obj;
        }

        private Object GetTravelRequestData(DataRow dc)
        {
            dynamic jsonObj = JObject.Parse(dc["JsonData"].ToString());
            JArray ja = (JArray)JsonConvert.DeserializeObject(jsonObj["TravelRequestTwoDetails"].ToString());
            List<TravelRequestDetailsModel> list = new List<TravelRequestDetailsModel>();
            if (ja.Count > 0)
            {
                for (int i = 0; i < ja.Count; i++)
                {
                    TravelRequestDetailsModel item = new TravelRequestDetailsModel();
                    item.类型 = ja[i]["Type"].ToString();
                    item.描述 = ja[i]["Destination"].ToString();
                    item.金额 = ja[i]["Amount"].ToString();
                    list.Add(item);
                }
            }
            TravelRequestModel obj = new TravelRequestModel();
            obj.流程单号 = dc["ProcessFolio"].ToString();
            obj.差旅类型 = jsonObj["Type"].ToString();
            obj.本位币 = jsonObj["BaseCurrency"].ToString();
            obj.外出日期 = Convert.ToDateTime(jsonObj["DepartureDate"]).ToString("yyyy-MM-dd");
            obj.返回日期 = Convert.ToDateTime(jsonObj["ReturnDate"]).ToString("yyyy-MM-dd");
            obj.出发地 = jsonObj["PlaceOfDepartureCountry"].ToString() + "-" + jsonObj["PlaceOfDepartureCountryCity"].ToString();
            obj.目的地 = jsonObj["DestinationCountry"].ToString() + "-" + jsonObj["DestinationCity"].ToString();
            obj.事由 = jsonObj["Reason"].ToString();
            obj.总计金额 = jsonObj["TotalCost"].ToString();
            obj.详细 = list;
            return obj;
        }

        private string GetFolioHeader(string folio)
        {
            return folio.Substring(0, 2);
        }

    }
}