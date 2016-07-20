using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TasksWeb.Help;
using TasksWeb.Models;

namespace TasksWeb.Controllers
{
    public class K2EventController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string QuanShiOASendAPIUrl = ConfigurationManager.AppSettings["QuanShiOASendAPIUrl"];

        // POST api/<controller>
        [HttpPost]
        [ActionName("PostTask")]
        public void PostTask([FromBody]WorkItemArgs work)
        {
            logger.Warn("有任务了，内容：" + JsonConvert.SerializeObject(work));
            try
            {
                string[] arrUrl = work.Task.Data.Split('?');
                string sn = null, sharedUser = null, action = null;
                DateTime thisDate = DateTime.Now;
                if (arrUrl.Length > 1)
                {
                    string queryString = arrUrl[1];
                    NameValueCollection col = TaskPush.GetQueryString(queryString);
                    sn = col["SN"];
                    sharedUser = col["SharedUser"];
                }
                else
                {
                    logger.Warn("获取不到SN号，Url：" + work.Task.Data);
                }
                if (work.Actions.Length > 0)
                {
                    for (int i = 0; i < work.Actions.Length; i++)
                    {
                        action += work.Actions[i] + ",";
                    }
                    action = action.Trim(',');
                }
                foreach (var item in work.Users)
                {

                    TaskDto task = new TaskDto()
                    {
                        Actions = action,
                        ActivityName = work.Task.ActivityName,
                        BusinessData = "",
                        CreationTime = thisDate,
                        Data = work.Task.Data,
                        Destination = item.Name,
                        Folio = work.Task.Folio,
                        Originator = work.Task.Originator,
                        ParamsTable = work.Task.ParamsTable,
                        ProcessName = work.Task.ProcessName,
                        ProcInstID = work.Task.ProcInstID,
                        SendTime = DateTime.MaxValue,
                        SharedUser = sharedUser,
                        SN = sn,
                        State = work.Task.State,
                        Status = TaskPushStatus.Create.ToString(),
                        StartDate = work.Task.StartDate
                    };
                    TaskPush.SaveTask(task);
                    var stask = Task.Run(() =>
                    {
                        return TaskPush.QuanShiPush(task, work.Actions, logger);
                    });
                    //TaskPush.QuanShiPush(task, work.Actions, logger);
                }
                logger.Warn("推送，保存推送任务成功！" + JsonConvert.SerializeObject(work));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }



        // POST api/<controller>
        [HttpGet]
        [ActionName("Get")]
        public string Get(int id = 0)
        {
            return "value" + id;
        }
    }
}