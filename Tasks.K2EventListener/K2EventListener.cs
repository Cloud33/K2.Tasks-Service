using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using log4net;
using log4net.Config;
using SourceCode.Workflow.Runtime;
using SourceCode.KO.Eventing;
using SourceCode.KO;
using System.Configuration;
using System.Reflection;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;
using System.Collections;
using Newtonsoft.Json;
using SourceCode.SmartObjects.Client;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"Tasks.K2EventListener.dll.config", Watch = true)]

namespace Tasks.K2EventListener
{
    /// <summary>
    /// K2事件侦听器
    /// <remarks>新实现，将替代TBEventListener</remarks>
    /// </summary>
    public class K2EventListener : IK2Service
    {
        private static ILog _log;

        /// <summary>
        /// 用于标识引擎，如：k2,aZaaS
        /// </summary>
        private static string State = "k2";
        //外部用于接收事件的服务
        private static readonly string K2EventReceiverUrl;
        private static readonly string smoConnectionString;
        private static readonly string Enviroment;
        //事件队列
        private static Queue<object> _argsQueue;
        private static bool _flag = false;
        private static object _lock = new object();

        static K2EventListener()
        {
            _argsQueue = new Queue<object>();
            K2EventReceiverUrl = ConfigurationManager.AppSettings["K2EventReceiverUrl"];
            smoConnectionString = ConfigurationManager.AppSettings["K2SmObjServer"];
            Enviroment = ConfigurationManager.AppSettings["Enviroment"];
            //使用log4net
            try
            {
                _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                //XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tasks.K2EventListener.dll.config"));
            }
            catch
            {
                //throw new Exception("KCloud.Workflow.K2Events.log4net.config not load.");
            }
            _log = LogManager.GetLogger(typeof(K2EventListener));
            _log.Info("static K2EventListener start log.");
        }
        public K2EventListener()
        {
            _log.Info("public K2EventListener start log.");
        }

        //HACK:发送消息一律使用此方法进行队列单线程异步发送
        private void SendArgs(object args)
        {
            _argsQueue.Enqueue(args);
            DoSendArgsTask();
        }
        private void DoSendArgsTask()
        {
            if (_flag)
                return;
            lock (_lock)
            {
                if (_flag)
                    return;
                _flag = true;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(o =>
            {
                object args = null;
                try
                {
                    while (_argsQueue.Count > 0)
                    {
                        args = _argsQueue.Dequeue();
                        if (args is WorkItemArgs)
                        {
                            PostWebRequest("PostTask", JsonConvert.SerializeObject(args), Encoding.UTF8);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Error("DoSendArgsTask Error At " + args, e);
                }
                finally { _flag = false; }
            });
        }
        private void CallK2EventService(string method, string user, object args)
        {
            //var query = string.Empty;

            //try
            //{
            //    using (var wc = new WebClient() { Encoding = Encoding.UTF8 })
            //    {
            //        query = "&state=" + State
            //            + "&user=" + HttpUtility.UrlEncode(user, Encoding.UTF8)
            //            + "&args="
            //            + HttpUtility.UrlEncode(Util.JsonSerialize(args, args.GetType()), Encoding.UTF8);
            //        _log.Info(query);
            //        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //        this.Handle(wc, args);
            //        wc.UploadData(K2EventReceiverUrl + "/" + method, Encoding.UTF8.GetBytes(query));

            //    }
            //}
            //catch (WebException e)
            //{
            //    using (var r = new StreamReader(e.Response.GetResponseStream()))
            //        throw new Exception("Query=" + query + "|Error=" + r.ReadToEnd(), e);
            //}
            //finally
            //{
            //    _log.InfoFormat("[K2Event]method={0}|user={1}|data={2}", method, user, query);
            //}
        }
        private static void PostWebRequest(string method, string paramData, Encoding dataEncode)
        {
            try
            {
                _log.Info("static PostWebRequest start");
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(K2EventReceiverUrl + "/" + method));
                webReq.Method = "POST";
                webReq.ContentType = "application/json;charset=UTF-8";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                var ws = webReq.GetResponseAsync();
                //StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode == null ? Encoding.Default : dataEncode);
                //sr.Close();
                //response.Close();
                newStream.Close();
                _log.Info("static PostWebRequest Success!");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                if (Enviroment == "Development") _log.InfoFormat("[K2Event]method={0}|data={1}", method, paramData);
            }
        }

        //private void Handle(WebClient wc, Object args)
        //{
        //    var bas = args as BaseArgs;
        //    if (bas != null)
        //    {
        //        var authContext = bas.GetAuthContext();
        //        wc.Headers.Add("AuthContext", Convert.ToBase64String(Encoding.UTF8.GetBytes(authContext.ToString())));
        //    }
        //}

        #region IK2Service 成员
        public void Start(XmlNode Setup)
        {
            try
            {
                _log.Info("Server listener start ");

                ActionRightsEvents.OnWorklistItemAdded += new WorklistItemAddedDelegate(ActionRightsEvents_OnWorklistItemAdded);
                //ActivityInstanceEvents.OnActivityCompleted += new ActivityCompletedDelegate(ActivityInstanceEvents_OnActivityCompleted);
                //ActivityInstanceEvents.OnActivityStarted += new ActivityStartedDelegate(ActivityInstanceEvents_OnActivityStarted);
                //ActivityEscalationRuleEvents.OnActivityEscalationInstanceActive += new ActivityEscalationInstanceActiveDelegate(ActivityEscalationRuleEvents_OnActivityEscalationInstanceActive);
                //ProcessEscelationRuleEvents.OnProcessEscalationInstanceActive += new ProcessEscalationInstanceActiveDelegate(ProcessEscelationRuleEvents_OnProcessEscalationInstanceActive);
                //ProcessInstanceEvents.OnProcessCompleted += new ProcessCompletedDelegate(ProcessInstanceEvents_OnProcessCompleted);
                //K2ServerSystemEvents.OnLicenseReminderMessage += new K2SystemEventDelegate(K2ServerSystemEvents_OnLicenseReminderMessage);

                _log.Info("Server listener start finished.");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        public void Stop()
        {
        }
        #endregion

        #region 事件逻辑
        //License提醒事件
        void K2ServerSystemEvents_OnLicenseReminderMessage(string SystemMessage)
        {
            _log.Info("LicenseReminder：" + SystemMessage);
        }

        //流程结束事件
        void ProcessInstanceEvents_OnProcessCompleted(ProcessInstance ProcInst)
        {
            //try
            //{
            //    Hashtable paramsTable = GetDataFields(ProcInst);
            //    this.SendArgs(new ProcessCompletedArgs(ProcInst.Originator.Name, ProcInst.Process.SetID, ProcInst.ID, (int)ProcInst.Status, ProcInst.Process.Name, ProcInst.Process.FullName, ProcInst.Folio, paramsTable));
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("OnProcessCompleted Error at process " + ProcInst.ID + "and status=" + ProcInst.Status, ex);
            //}
        }
        //流程升级事件
        void ProcessEscelationRuleEvents_OnProcessEscalationInstanceActive(ProcessEscalationInstance EscInst)
        {
            //ProcessEscRuleArgs procEscRuleArgs = null;
            //try
            //{
            //    Hashtable paramsTable = GetDataFields(EscInst.ProcessInstance);
            //    procEscRuleArgs = new ProcessEscRuleArgs(
            //        EscInst.ProcessInstance.Originator.Name,
            //        EscInst.ProcessInstance.Process.SetID,
            //        EscInst.ProcessInstance.ID,
            //        EscInst.ProcessInstance.Process.Name,
            //        EscInst.ProcessInstance.Process.FullName,
            //        EscInst.ProcessInstance.Folio,
            //        paramsTable);

            //    this.SendArgs(procEscRuleArgs);
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("OnProcessEscalationInstanceActive Error", ex);
            //}
        }
        //节点升级事件
        void ActivityEscalationRuleEvents_OnActivityEscalationInstanceActive(ActivityEscalationInstance EscInst)
        {
            //ActEscRuleArgs actEscRuleArgs = null;
            //try
            //{
            //    Hashtable paramsTable = GetDataFields(EscInst.ProcessInstance);
            //    actEscRuleArgs = new ActEscRuleArgs(new string[] { },
            //        this.ParseUser(EscInst.ProcessInstance.Originator.Name),
            //        EscInst.ProcessInstance.Process.SetID,
            //        EscInst.ProcessInstance.ID,
            //        EscInst.ProcessInstance.Process.Name,
            //        EscInst.ProcessInstance.Process.FullName,
            //        EscInst.ActivityInstance.Activity.Name,
            //        EscInst.ProcessInstance.Folio,
            //        paramsTable);

            //    if (EscInst.ActivityInstance.DataFields.Count > 0)
            //    {
            //        foreach (DataField df in EscInst.ActivityInstance.DataFields)
            //        {
            //            if (df.Name == Statics.NOTIFYUSERSKEY)
            //            {
            //                if (paramsTable.Contains(Statics.NOTIFYUSERSKEY))
            //                {
            //                    paramsTable.Remove(Statics.NOTIFYUSERSKEY);
            //                }
            //                paramsTable.Add(Statics.NOTIFYUSERSKEY, df.Value);
            //            }
            //        }
            //    }
            //    this.SendArgs(actEscRuleArgs);
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("OnActivityEscalationInstanceActive Error", ex);
            //}
        }
        //任务创建事件
        void ActionRightsEvents_OnWorklistItemAdded(WorklistItemInstanceContext worklistitem)
        {
            _log.InfoFormat("Start OnWorklistItemAdded");
            //System.Diagnostics.EventLog.WriteEntry("K2EventListener", "K2EventListener ActionRightsEvents_OnWorklistItemAdded Started!Data：" + JsonConvert.SerializeObject(worklistitem), System.Diagnostics.EventLogEntryType.Warning);
            WorkItemArgs workItemArgs = null;
            try
            {
                //string[] users = new string[worklistitem.EventDestinations.Count];
                //int i = 0;
                if (worklistitem.EventDestinations.Count > 0)
                {
                    List<TaskUser> users = new List<TaskUser>();

                    string[] actions = worklistitem.EventDestinations[0].AllowedActions.ToString().Split(',');
                    foreach (SourceCode.KO.Destination dest in worklistitem.EventDestinations)
                    {
                        users.AddRange(GetUsers(dest));
                    }

                    Hashtable paramsTable = GetDataFields(worklistitem.ClientEventContext.ProcessInstance);
                    TaskModel task = new TaskModel()
                    {
                        ProcInstID = worklistitem.ProcInstID,
                        Folio = worklistitem.Folio,
                        Originator = this.ParseUser(worklistitem.ClientEventContext.ProcessInstance.Originator.Name),
                        ActivityName = worklistitem.ActName,
                        ProcessName = worklistitem.ProcessName,
                        Data = worklistitem.Data,
                        State = State,
                        ActInstDestID = worklistitem.ActInstDestID,
                        ParamsTable = JsonConvert.SerializeObject(paramsTable),
                        StartDate = worklistitem.ClientEventContext.ProcessInstance.StartDate
                    };
                    workItemArgs = new WorkItemArgs()
                    {
                        Actions = actions,
                        Users = users,
                        Task = task
                    };
                    if (Enviroment == "Development") _log.InfoFormat("Start SendArgs At OnWorklistItemAdded By process {0} and users={1}"
                        , worklistitem.ProcInstID
                        , JsonConvert.SerializeObject(users));
                    this.SendArgs(workItemArgs);
                }
                else
                {
                    _log.Error(new Exception("SourceCode.KO.Destination Count ==0 Error"));
                }

            }
            catch (Exception ex)
            {
                _log.Error("OnWorklistItemAdded Error", ex);
            }
        }
        //节点完成事件
        void ActivityInstanceEvents_OnActivityCompleted(ActivityInstance ActInst)
        {
            //ActivityCompletedArgs args = null;
            //try
            //{
            //    Hashtable paramsTable = GetDataFields(ActInst.ProcessInstance);
            //    if (ActInst.DataFields.Count > 0)
            //    {
            //        foreach (DataField df in ActInst.DataFields)
            //        {
            //            if (df.Name == Statics.NOTIFYUSERSKEY)
            //            {
            //                if (paramsTable.Contains(Statics.NOTIFYUSERSKEY))
            //                {
            //                    paramsTable.Remove(Statics.NOTIFYUSERSKEY);
            //                }
            //                paramsTable.Add(Statics.NOTIFYUSERSKEY, df.Value);
            //            }
            //        }
            //    }

            //    args = new ActivityCompletedArgs(
            //        this.ParseUser(ActInst.ProcessInstance.Originator.Name),
            //        ActInst.ProcessInstance.Process.SetID,
            //        ActInst.ProcessInstance.ID,
            //        ActInst.ProcessInstance.Process.Name,
            //        ActInst.ProcessInstance.Process.FullName,
            //        ActInst.Activity.Name,
            //        string.Empty,
            //        ActInst.ProcessInstance.Folio,
            //        paramsTable) { Users = this.ParseActivityUsers(ActInst) };

            //    this.SendArgs(args);
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("OnActivityCompleted Error", ex);
            //}
        }
        //节点开始事件
        void ActivityInstanceEvents_OnActivityStarted(ActivityInstance ActInst)
        {
            //ActivityStartedArgs args = null;
            //try
            //{
            //    Hashtable paramsTable = GetDataFields(ActInst.ProcessInstance);
            //    if (ActInst.FinishLineInstance == null || ActInst.FinishLineInstance.StartActivityInstance == null)
            //        return;
            //    ActivityInstance finishActInst = ActInst.FinishLineInstance.StartActivityInstance;
            //    if (finishActInst.DataFields.Count > 0)
            //    {
            //        foreach (DataField df in finishActInst.DataFields)
            //        {
            //            if (df.Name == Statics.NOTIFYUSERSKEY)
            //            {
            //                if (paramsTable.Contains(Statics.NOTIFYUSERSKEY))
            //                {
            //                    paramsTable.Remove(Statics.NOTIFYUSERSKEY);
            //                }
            //                paramsTable.Add(Statics.NOTIFYUSERSKEY, df.Value);
            //            }
            //        }
            //    }

            //    args = new ActivityStartedArgs(
            //        this.ParseUser(finishActInst.ProcessInstance.Originator.Name),
            //        finishActInst.ProcessInstance.Process.SetID,
            //        finishActInst.ProcessInstance.ID,
            //        finishActInst.ProcessInstance.Process.Name,
            //        finishActInst.ProcessInstance.Process.FullName,
            //        //finishActInst.Activity.Name,
            //        ActInst.Activity.Name,
            //        ActInst.FinishLineInstance.Line.Name,
            //        finishActInst.ProcessInstance.Folio,
            //        paramsTable);

            //    this.SendArgs(args);
            //}
            //catch (Exception ex)
            //{
            //    _log.Error("OnActivityStarted Error", ex);
            //}
        }
        //转换DataFields
        private static Hashtable GetDataFields(ProcessInstance procInst)
        {
            var paramsTable = new Hashtable();
            foreach (DataField dataField in procInst.DataFields)
            {
                if (dataField.Value.GetType() == typeof(DateTime))
                {
                    _log.WarnFormat("发现DateTime类型的流程变量，请尽快修改避免使用，Key={0}|Value={1}|ProcInstId={2}"
                        , dataField.Name
                        , dataField.Value
                        , procInst.ID);

                    var time = (DateTime)dataField.Value;
                    if (time > DateTime.MaxValue)
                        paramsTable.Add(dataField.Name, DateTime.MaxValue);
                    if (time < DateTime.MinValue)
                        paramsTable.Add(dataField.Name, DateTime.MinValue);
                }
                else
                    paramsTable.Add(dataField.Name, dataField.Value);
            }
            return paramsTable;
        }
        #endregion

        //private string[] ParseActivityUsers(ActivityInstance activity)
        //{
        //    var destinations = activity.Destinations;
        //    var users = new List<string>();
        //    if (destinations != null)
        //    {
        //        foreach (ActivityInstanceDestination dest in destinations)
        //        {
        //            if (dest.User != null)
        //                users.Add(this.ParseUser(dest.User.Name));
        //            if (dest.Destinations != null)
        //                foreach (Destination d in dest.Destinations)
        //                    users.Add(this.ParseUser(d.Name));
        //        }
        //    }
        //    if (activity.WorklistSlots != null)
        //    {
        //        foreach (Slot s in activity.WorklistSlots)
        //        {
        //            if (s.User != null)
        //                users.Add(this.ParseUser(s.User.Name));
        //        }
        //    }
        //    return users.ToArray();
        //}
        private string ParseUser(string k2User)
        {
            if (string.IsNullOrEmpty(k2User))
                return k2User;
            var temp = k2User.Split(':');
            return temp.Length > 1 ? temp[1] : k2User;
        }


        #region Smobj
        private List<TaskUser> GetUsers(SourceCode.KO.Destination dest)
        {
            //System.Diagnostics.EventLog.WriteEntry("K2EventListener", "K2EventListener GetUsers Started!Data：" + JsonConvert.SerializeObject(arr), System.Diagnostics.EventLogEntryType.Warning);
            //System.Diagnostics.EventLog.WriteEntry("K2EventListener", "smoConnectionString Data：" + smoConnectionString + ",K2EventReceiverUrl：" + K2EventReceiverUrl, System.Diagnostics.EventLogEntryType.Error);
            _log.Info("Start GetUsers");
            List<TaskUser> users = new List<TaskUser>();
            try
            {
                //Create a SO Server Client Object
                SmartObjectClientServer soServer = new SmartObjectClientServer();
                //Open the connection to the K2 Server
                soServer.CreateConnection();
                soServer.Connection.Open(smoConnectionString);
                //Get a handle to the 'Employee' SO
                SmartObject soUMUser = soServer.GetSmartObject("UMUser");
                if (dest.Type == DestinationType.User)
                {
                    soUMUser.MethodToExecute = "Get_Users";
                    string[] arr = dest.Name.Split(':');
                    if (arr.Length > 1)
                    {
                        soUMUser.ListMethods["Get_Users"].InputProperties["Label_Name"].Value = arr[0];
                        soUMUser.ListMethods["Get_Users"].InputProperties["Name"].Value = arr[1];
                        //Execute GetList Method, and put the result to a SmartObjectList
                        SmartObjectList smoList = soServer.ExecuteList(soUMUser);
                        //Iterate the SmartObject List
                        foreach (SmartObject soDetail in smoList.SmartObjectsList)
                        {
                            TaskUser user = new TaskUser()
                            {
                                Name = soDetail.Properties["Name"].Value.ToString(),
                                Email = soDetail.Properties["Email"].Value.ToString(),
                                Manager = soDetail.Properties["Manager"].Value.ToString()
                            };
                            users.Add(user);
                        }
                        soServer.Connection.Close();
                    }
                    soServer.Connection.Close();
                    if (Enviroment == "Development") _log.Info("GetUsers Return DestinationType.User Data：" + JsonConvert.SerializeObject(users));
                    return users;
                }
                else if (dest.Type == DestinationType.Group)
                {
                    soUMUser.MethodToExecute = "Get_Group_Users";
                    //Input Properties setting:
                    string[] arr = dest.Name.Split(':');
                    if (arr.Length > 1)
                    {
                        soUMUser.ListMethods["Get_Group_Users"].InputProperties["Group_Name"].Value = arr[1];
                        soUMUser.ListMethods["Get_Group_Users"].InputProperties["LabelName"].Value = arr[0];
                        //Execute GetList Method, and put the result to a SmartObjectList
                        SmartObjectList smoList = soServer.ExecuteList(soUMUser);
                        //Iterate the SmartObject List
                        foreach (SmartObject soDetail in smoList.SmartObjectsList)
                        {
                            TaskUser user = new TaskUser()
                            {
                                Name = soDetail.Properties["Name"].Value.ToString(),
                                Email = soDetail.Properties["Email"].Value.ToString(),
                                Manager = soDetail.Properties["Manager"].Value.ToString()
                            };
                            users.Add(user);
                        }
                        soServer.Connection.Close();
                    }
                    if (Enviroment == "Development") _log.Info("GetUsers Return DestinationType.Group Data：" + JsonConvert.SerializeObject(users));
                    return users;
                }
                else if (dest.Type == DestinationType.Queue)
                {
                    //Call the GetList Method
                    soUMUser.MethodToExecute = "Get_Role_Users";
                    //Input Properties setting:
                    soUMUser.ListMethods["Get_Role_Users"].InputProperties["Role_Name"].Value = dest.Name;
                    //Execute GetList Method, and put the result to a SmartObjectList
                    SmartObjectList smoList = soServer.ExecuteList(soUMUser);
                    //Iterate the SmartObject List
                    foreach (SmartObject soDetail in smoList.SmartObjectsList)
                    {
                        TaskUser user = new TaskUser()
                        {
                            Name = soDetail.Properties["Name"].Value.ToString(),
                            Email = soDetail.Properties["Email"].Value.ToString(),
                            Manager = soDetail.Properties["Manager"].Value.ToString()
                        };
                        users.Add(user);
                    }
                    soServer.Connection.Close();
                    if (Enviroment == "Development") _log.Info("GetUsers Return DestinationType.Queue Data：" + JsonConvert.SerializeObject(users));
                    //System.Diagnostics.EventLog.WriteEntry("K2EventListener", "K2EventListener Return GetUsers Data：" + JsonConvert.SerializeObject(users), System.Diagnostics.EventLogEntryType.Error);
                    return users;
                }
                else
                {
                    soServer.Connection.Close();
                    if (Enviroment == "Development") _log.InfoFormat("GetUsers　Return null Data：" + JsonConvert.SerializeObject(users));
                    return users;
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.EventLog.WriteEntry("K2EventListener", "K2EventListener Error Data：" + JsonConvert.SerializeObject(ex), System.Diagnostics.EventLogEntryType.Error);
                _log.Error(ex);
            }
            return users;
        }
        #endregion
    }
}
