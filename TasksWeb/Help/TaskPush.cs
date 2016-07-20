using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TasksWeb.Models;

namespace TasksWeb.Help
{
    public static class TaskPush
    {
        private static readonly string TasksDB = ConfigurationManager.ConnectionStrings["Tasks"].ConnectionString;
        private static readonly string AZaaSFrameworkDB = ConfigurationManager.ConnectionStrings["aZaaSFramework"].ConnectionString;
        private static readonly string QuanShiLoginAPIUrl = ConfigurationManager.AppSettings["QuanShiLoginAPIUrl"];
        private static readonly string QuanShiOASendAPIUrl = ConfigurationManager.AppSettings["QuanShiOASendAPIUrl"];
        private static readonly string QuanShiOASendChangeAPIUrl = ConfigurationManager.AppSettings["QuanShiOASendChangeAPIUrl"];

        private static readonly string TaskUrl = ConfigurationManager.AppSettings["TaskUrl"];
        private static readonly string IsEmail = ConfigurationManager.AppSettings["IsEmail"];
        private static readonly string WindowDomain = ConfigurationManager.AppSettings["WindowDomain"];
        private static readonly string QuanShiAccount = ConfigurationManager.AppSettings["QuanShiAccount"];
        private static readonly string QuanShiPwb = ConfigurationManager.AppSettings["QuanShiPwb"];
        static SemaphoreSlim _sem = new SemaphoreSlim(30);
        public static void SaveTask(TaskDto task)
        {
            using (SqlConnection conn = new SqlConnection(TasksDB))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("INSERT INTO [dbo].[Tasks]([ProcInstID],[Folio],[Originator],[SharedUser],[ActivityName],[ProcessName],[Destination],[SN],[BusinessData],[Data],[ParamsTable],[Actions],[Status],[State],[CreationTime],[SendTime]) values(@ProcInstID,@Folio,@Originator,@SharedUser,@ActivityName,@ProcessName,@Destination,@SN,@BusinessData,@Data,@ParamsTable,@Actions,@Status,@State,@CreationTime,@SendTime)", conn);
                //@ProcInstID,@Folio,@ActivityName,@ProcessName,@Destination,@SN,@BusinessData,@Data,@Status,@State,@CreationTime,@SendTime
                comm.Parameters.Add(new SqlParameter("@ProcInstID", SqlNull(task.ProcInstID)));
                comm.Parameters.Add(new SqlParameter("@Folio", SqlNull(task.Folio)));
                comm.Parameters.Add(new SqlParameter("@Originator", SqlNull(task.Originator)));
                comm.Parameters.Add(new SqlParameter("@SharedUser", SqlNull(task.SharedUser)));
                comm.Parameters.Add(new SqlParameter("@ActivityName", SqlNull(task.ActivityName)));
                comm.Parameters.Add(new SqlParameter("@ProcessName", SqlNull(task.ProcessName)));
                comm.Parameters.Add(new SqlParameter("@Destination", SqlNull(task.Destination)));
                comm.Parameters.Add(new SqlParameter("@SN", SqlNull(task.SN)));
                comm.Parameters.Add(new SqlParameter("@BusinessData", task.BusinessData));
                comm.Parameters.Add(new SqlParameter("@Data", SqlNull(task.Data)));
                comm.Parameters.Add(new SqlParameter("@ParamsTable", SqlNull(task.ParamsTable)));
                comm.Parameters.Add(new SqlParameter("@Actions", SqlNull(task.Actions)));
                comm.Parameters.Add(new SqlParameter("@Status", SqlNull(task.Status)));
                comm.Parameters.Add(new SqlParameter("@State", SqlNull(task.State)));
                comm.Parameters.Add(new SqlParameter("@CreationTime", SqlNull(task.CreationTime)));
                comm.Parameters.Add(new SqlParameter("@SendTime", SqlNull(task.SendTime)));
                int i = comm.ExecuteNonQuery();
                conn.Close();
            }
        }
        public static void SaveMsgId(QuanShiMsgModel msg)
        {
            using (SqlConnection conn = new SqlConnection(TasksDB))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("INSERT INTO [dbo].[QuanShiMsg]([sysId],[msgId]) values(@sysId,@msgId)", conn);
                //@ProcInstID,@Folio,@ActivityName,@ProcessName,@Destination,@SN,@BusinessData,@Data,@Status,@State,@CreationTime,@SendTime
                comm.Parameters.Add(new SqlParameter("@sysId", SqlNull(msg.sysId)));
                comm.Parameters.Add(new SqlParameter("@msgId", SqlNull(msg.msgId)));
                int i = comm.ExecuteNonQuery();
                conn.Close();
            }
        }


        public static string GetMsgId(string sysId)
        {
            using (SqlConnection conn = new SqlConnection(TasksDB))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("SELECT msgId FROM [dbo].[QuanShiMsg] WHERE sysId=@sysId", conn);
                comm.Parameters.Add(new SqlParameter("@sysId", sysId));
                string msgId = comm.ExecuteScalar() as string;
                conn.Close();
                return msgId;
            }
        }

        public static DataTable GetBusinessJsonData(int procInstID)
        {
            using (SqlConnection conn = new SqlConnection(AZaaSFrameworkDB))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(@"SELECT fh.ProcessFolio,fc.JsonData FROM [dbo].[ProcessFormHeader] fh 
                        LEFT JOIN [dbo].[ProcessFormContent] fc ON fc.FormID=fh.FormID
                        WHERE fh.ProcInstID=@ProcInstID", conn);
                comm.Parameters.Add(new SqlParameter("@ProcInstID", procInstID));
                SqlDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable myDataTable = new DataTable();
                myDataTable.Load(dr);
                return myDataTable;
            }
        }



        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest request = null;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.Method = "POST";
                request.UserAgent = DefaultUserAgent;
                request.ContentType = "application/json;charset=UTF-8";

                request.ContentLength = byteArray.Length;
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode == null ? Encoding.Default : dataEncode);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public static string CreateQuanShiToKen()
        {
            LoginModel login = new LoginModel()
            {
                username = QuanShiAccount,
                password = QuanShiPwb,
                role = 0
            };
            return PostWebRequest(QuanShiLoginAPIUrl, JsonConvert.SerializeObject(login), Encoding.UTF8);
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        static public object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
        #region 解码URL.
        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString)
        {
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        private static NameValueCollection GetQueryString(string queryString, Encoding encoding, bool isEncoded)
        {
            queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }
        #endregion

        public static int QuanShiPush(TaskDto task, string[] actions, log4net.ILog logger)
        {
            //_sem.Wait();
            try
            {
                //var CreateQuanShiToKenAsync = TaskPush.CreateQuanShiToKenAsync();
                //_sem.Wait();
                string data = TaskPush.CreateQuanShiToKen();
                LoginRequestModel re = new LoginRequestModel();
                re = JsonConvert.DeserializeObject<LoginRequestModel>(data);

                if (re.crrorCode == 0)
                {
                    OANewsModel oa = new OANewsModel();
                    oa.username = re.data.username;
                    oa.token = re.data.token;
                    OANewsDataModel oaData = new OANewsDataModel();
                    List<string> users = new List<string>();
                    if (string.IsNullOrEmpty(IsEmail))
                    {
                        users.Add(task.Destination.Replace(WindowDomain + @"\", ""));
                    }
                    else
                    {
                        users.Add(task.Destination.Replace(WindowDomain + @"\", "") + IsEmail);
                    }
                    oaData.toUsers = users.ToArray();
                    oaData.toPartyIds = new int[] { };
                    oaData.appId = 136;
                    oaData.title = task.Folio;
                    oaData.color = "red";
                    oaData.status = 11;
                    oaData.elements = new List<object>();
                    string sharedUser = string.IsNullOrEmpty(task.SharedUser) ? "null" : task.SharedUser;
                    oaData.detailURL = string.Format(TaskUrl + "task/{0}/{1}/{2}/{3}", task.SN, task.ProcInstID, System.Web.HttpUtility.UrlEncode(task.Destination), sharedUser);
                    oaData.detailAuth = 1;
                    //申请信息
                    OANewsDataTextRichModel text1 = new OANewsDataTextRichModel();
                    text1.type = "text";
                    //text1.status = 11;
                    text1.content = new List<OANewsDataTextContentModel>();
                    OANewsDataTextContentModel content = new OANewsDataTextContentModel() { size = 2, bold = 1, text = "申请信息" };
                    text1.content.Add(content);
                    oaData.elements.Add(text1);

                    OANewsDataTextModel text6 = new OANewsDataTextModel() { type = "text", content = " " };
                    oaData.elements.Add(text6);

                    //申请人
                    OANewsDataTextRichModel text2 = new OANewsDataTextRichModel();
                    text2.type = "text";
                    //text2.status = 11;
                    text2.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { bold = 1, text = "申请人：" };
                    text2.content.Add(content);
                    content = new OANewsDataTextContentModel() { bold = 0, text = task.Originator };
                    text2.content.Add(content);
                    oaData.elements.Add(text2);

                    OANewsDataTextRichModel text3 = new OANewsDataTextRichModel();
                    text3.type = "text";
                    //text3.status = 11;
                    text3.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { bold = 1, text = "申请时间：" };
                    text3.content.Add(content);
                    content = new OANewsDataTextContentModel() { bold = 0, text = task.StartDate.ToString("yyyy-MM-dd HH:mm") };
                    text3.content.Add(content);
                    oaData.elements.Add(text3);

                    OANewsDataTextRichModel text4 = new OANewsDataTextRichModel();
                    text4.type = "text";
                    //text4.status = 11;
                    text4.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { bold = 1, text = "当前环节：" };
                    text4.content.Add(content);
                    content = new OANewsDataTextContentModel() { bold = 0, text = task.ActivityName };
                    text4.content.Add(content);
                    oaData.elements.Add(text4);
                    OANewsDataTextModel text7 = new OANewsDataTextModel() { type = "text", content = " " };
                    oaData.elements.Add(text7);
                    //操作
                    OANewsDataTextRichModel text5 = new OANewsDataTextRichModel();
                    text5.type = "text";
                    text5.status = 11;
                    text5.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { size = 2, bold = 1, text = "操作" };
                    text5.content.Add(content);
                    oaData.elements.Add(text5);

                    //操作
                    OANewsDataTextRichModel text11 = new OANewsDataTextRichModel();
                    text11.type = "text";
                    text11.status = 12;
                    text11.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { size = 2, bold = 1, text = "操作" };
                    text11.content.Add(content);
                    oaData.elements.Add(text11);

                    OANewsDataTextRichModel text10 = new OANewsDataTextRichModel();
                    text10.type = "text";
                    text10.status = 12;
                    text10.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { color = "#FF0000", bold = 1, text = "审批过程中出现错误，请重试或联系管理员处理！" };
                    text10.content.Add(content);
                    oaData.elements.Add(text10);
                    // status=11
                    OANewsDataActionModel action = new OANewsDataActionModel();
                    action.type = "action";
                    action.status = 11;
                    action.buttons = new List<OANewsDataButtonModel>();

                    Guid sysId = Guid.NewGuid();
                    foreach (var item in actions)
                    {

                        string actionUrl = string.Format(TaskUrl + "quanShiExecuteTask?ActionName={0}&SN={1}&Destination={2}&SharedUser={3}&ProcInstID={4}", System.Web.HttpUtility.UrlEncode(item), task.SN, System.Web.HttpUtility.UrlEncode(task.Destination), sharedUser, sysId);
                        OANewsDataButtonModel button = new OANewsDataButtonModel() { title = item, url = actionUrl };
                        action.buttons.Add(button);
                    }
                    oaData.elements.Add(action);

                    // status=11
                    action = new OANewsDataActionModel();
                    action.type = "action";
                    action.status = 12;
                    action.buttons = new List<OANewsDataButtonModel>();
                    foreach (var item in actions)
                    {

                        string actionUrl = string.Format(TaskUrl + "quanShiExecuteTask?ActionName={0}&SN={1}&Destination={2}&SharedUser={3}&ProcInstID={4}", System.Web.HttpUtility.UrlEncode(item), task.SN, System.Web.HttpUtility.UrlEncode(task.Destination), sharedUser, sysId);
                        OANewsDataButtonModel button = new OANewsDataButtonModel() { title = item, url = actionUrl };
                        action.buttons.Add(button);
                    }
                    oaData.elements.Add(action);



                    //状态
                    OANewsDataTextRichModel text8 = new OANewsDataTextRichModel();
                    text8.type = "text";
                    text8.status = 1;
                    text8.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { bold = 1, color = "#808080", text = "状态：" };
                    text8.content.Add(content);
                    content = new OANewsDataTextContentModel() { bold = 0, color = "#FF0000", text = "已处理" };
                    text8.content.Add(content);
                    oaData.elements.Add(text8);

                    OANewsDataTextRichModel text9 = new OANewsDataTextRichModel();
                    text9.type = "text";
                    text9.status = 9;
                    text9.content = new List<OANewsDataTextContentModel>();
                    content = new OANewsDataTextContentModel() { bold = 1, color = "#808080", text = "状态：" };
                    text9.content.Add(content);
                    content = new OANewsDataTextContentModel() { bold = 0, color = "#FF0000", text = "任务已经被处理" };
                    text9.content.Add(content);
                    oaData.elements.Add(text9);

                    oa.data = oaData;
                    string psp = JsonConvert.SerializeObject(oa);
                    //var CreateQuanShiOAAsync = TaskPush.CreateQuanShiOAAsync(psp);
                    string dataCheck = TaskPush.PostWebRequest(QuanShiOASendAPIUrl, psp, Encoding.UTF8);
                    JObject obj = JObject.Parse(dataCheck);
                    if (obj["errorCode"].ToString() == "0")
                    {
                        QuanShiMsgModel msg = new QuanShiMsgModel() { sysId = sysId.ToString(), msgId = obj["data"][0]["msgId"].ToString() };
                        SaveMsgId(msg);
                        logger.Info("QuanShiPush_任务推送请求成功，请求消息：" + psp + "；返回消息：" + dataCheck);

                    }
                    else
                    {
                        logger.Error("QuanShiPush_任务推送请求失败，请求消息：" + psp + "；返回消息：" + dataCheck);
                    }
                }
                //_sem.Release();
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error("QuanShiPush_请求失败，在执行过程中出现异常", ex);
                //_sem.Release();
                return 2;
            }
        }

        public static void QuanShiPushChange(string sysId, int newStatus, log4net.ILog logger)
        {
            string msgId = GetMsgId(sysId);
            try
            {
                string data = TaskPush.CreateQuanShiToKen();
                LoginRequestModel re = new LoginRequestModel();
                re = JsonConvert.DeserializeObject<LoginRequestModel>(data);
                if (re.crrorCode == 0)
                {
                    OANewsChangeModel ch = new OANewsChangeModel()
                    {
                        username = re.data.username,
                        token = re.data.token
                    };
                    OANewsChangeDataModel item = new OANewsChangeDataModel();
                    item.msgId = msgId;
                    item.newStatus = newStatus;
                    ch.data = item;
                    string psp = JsonConvert.SerializeObject(ch);
                    string dataCheck = TaskPush.PostWebRequest(QuanShiOASendChangeAPIUrl, psp, Encoding.UTF8);
                    JObject obj = JObject.Parse(dataCheck);
                    if (obj["errorCode"].ToString() == "0")
                    {
                        logger.Info("QuanShiPushChange_OA消息状态更新成功，请求数据：" + sysId + "，返回数据：" + dataCheck);
                    }
                    else
                    {
                        logger.Warn("QuanShiPushChange_OA消息状态更新失败，请求数据：" + sysId + "，返回数据：" + dataCheck);
                    }
                }
                else
                {
                    logger.Warn("QuanShiPushChange_管理员登录失败，请求数据：" + sysId + "，返回数据：" + data);
                }

            }
            catch (Exception ex)
            {
                logger.Error("QuanShiPushChange_Login请求失败，在执行过程中出现异常", ex);
            }
        }


        private static async Task<string> CreateQuanShiToKenAsync()
        {
            return await Task.Run(() =>
            {
                LoginModel login = new LoginModel()
                {
                    username = QuanShiAccount,
                    password = QuanShiPwb,
                    role = 0
                };
                return PostWebRequest(QuanShiLoginAPIUrl, JsonConvert.SerializeObject(login), Encoding.UTF8);
            });

        }

        private static async Task<string> CreateQuanShiOAAsync(string data)
        {
            return await Task.Run(() =>
            {
                return TaskPush.PostWebRequest(QuanShiOASendAPIUrl, data, Encoding.UTF8);
            });
        }
    }
}