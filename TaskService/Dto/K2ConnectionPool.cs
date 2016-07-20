using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public sealed class K2ConnectionPool
    {
        //private static string oServerName = System.Configuration.ConfigurationManager.AppSettings["K2Server"].ToString();
        private static string domain = string.Empty; //System.Configuration.ConfigurationManager.AppSettings["Domain"].ToString();
        private static string k2User = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["K2User"].ToString();
        private static string k2PassWord = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["K2PassWord"].ToString();
        private static string label = string.Empty;
        private static string clientPort = "5252";
        private static Random rndGen = new Random();
        private static int maxPoolSize = 40; //减少默认连接池数 pcithpx 20110309 [k2Init]

        /// <summary>
        /// 连接池是否已经构造
        /// （Wrap数组的构造）
        /// </summary>
        public static bool IsConstructed = false;

        private static int noPoolConnCount = 0;
        //非连接池的连接 pcithpx 20110728 [k2Init2]
        public static int NoPoolConnCount
        {
            get
            {
                return noPoolConnCount;
            }
        }
        private static K2ConnWrap[] k2ConnWrap; // 延迟声明 pcithpx 20110309 [k2Init]

        public static K2ConnWrap[] K2ConnWrap
        {
            get { return K2ConnectionPool.k2ConnWrap; }
            set { K2ConnectionPool.k2ConnWrap = value; }
        }

        /// <summary>
        /// 配置的K2服务器数组
        /// </summary>
        public static string[] aryServerName;

        /// <summary>
        /// 当前K2服务器的索引号 
        /// </summary>
        public static int curK2ServerIndex = 0;

        private static object oLock = new object();

        /// <summary>
        /// 清空连接池的初始化状态，恢复到最初的状态
        /// </summary>
        public static void ResetPool()
        {
            if (K2ConnectionPool.K2ConnWrap != null)
            {
                for (int i = 0; i < K2ConnectionPool.K2ConnWrap.Length; i++)
                {
                    K2ConnectionPool.K2ConnWrap[i].Status = 0;
                    K2ConnectionPool.K2ConnWrap[i].Initialized = false;

                    if (K2ConnectionPool.K2ConnWrap[i].K2Conn == null) continue;

                    try
                    {
                        K2ConnectionPool.K2ConnWrap[i].K2Conn.Close();
                        K2ConnectionPool.K2ConnWrap[i].K2Conn.Dispose();
                        K2ConnectionPool.K2ConnWrap[i].K2Conn = null;
                    }
                    finally { }
                }
            }

            IsConstructed = false;

        }

        public static void InitConfigValue(ServiceContext context)
        {
            domain = context[SettingVariable.WindowDomain];
            k2User = context[SettingVariable.LoginUser];
            k2PassWord = context[SettingVariable.LoginPassword];
            label = context[SettingVariable.SecurityLabelName];
            clientPort = context[SettingVariable.ClientPort];
        }

        /// <summary>
        /// 获取一个K2连接池
        /// </summary>
        /// <returns></returns>
        public static Connection GetConnection(ServiceContext context)
        {
            InitConfigValue(context);
            //todo:   
            lock (oLock)
            {
                InitK2ConnPool(context);
            }

            lock (oLock)
            {
                CheckK2ConnPool();
            }

            int seed = 0;
            int currPoolId = -1;

            lock (oLock)
            {
                seed = rndGen.Next(maxPoolSize);

                for (int i = seed; i < maxPoolSize; i++)
                {
                    if (k2ConnWrap[i].Initialized
                        && k2ConnWrap[i].Status == 0)
                    {
                        k2ConnWrap[i].Status = 1;
                        currPoolId = i;
                        break;
                    }
                }

                if (currPoolId == -1)
                {
                    for (int i = seed - 1; i >= 0; i--)
                    {
                        if (k2ConnWrap[i].Initialized
                            && k2ConnWrap[i].Status == 0)
                        {
                            k2ConnWrap[i].Status = 1;
                            currPoolId = i;
                            break;
                        }
                    }
                }
            }

            //todo:
            try
            {
                if (currPoolId != -1)
                {
                    k2ConnWrap[currPoolId].TimeOfGetting = DateTime.Now;
                    k2ConnWrap[currPoolId].StackOfGetting = getStackInfo();

                    k2ConnWrap[currPoolId].K2Conn.RevertUser();
                    return k2ConnWrap[currPoolId].K2Conn;
                }
                else
                {
                    Exception exp = null;
                    int hasNoInitializedIndex = -1;
                    lock (oLock)
                    {

                        for (int i = 0; i < k2ConnWrap.Length; i++)
                        {
                            if (k2ConnWrap[i].Initialized) continue;

                            K2ConnWrap[i].Status = 1;
                            k2ConnWrap[i].Initialized = true;

                            hasNoInitializedIndex = i;
                            break;
                        }
                    }

                    if (hasNoInitializedIndex == -1)
                    {
                        return GetNewK2Conn(context);
                    }

                    int tryCount = 0;
                    for (; tryCount < aryServerName.Length; tryCount++)
                    {
                        curK2ServerIndex = curK2ServerIndex % aryServerName.Length;

                        try
                        {
                            k2ConnWrap[hasNoInitializedIndex].TimeOfGetting = DateTime.Now;
                            k2ConnWrap[hasNoInitializedIndex].StackOfGetting = getStackInfo();
                            k2ConnWrap[hasNoInitializedIndex].InitConn(context, aryServerName[curK2ServerIndex]);
                            return k2ConnWrap[hasNoInitializedIndex].K2Conn;
                        }
                        catch (Exception e)
                        {
                            exp = e;
                            curK2ServerIndex++;
                            continue;
                        }

                    }

                    K2PoolLogger.logError(
                        "所有的K2服务器都无法连接。\r\n"
                        + exp.Message + "\r\n" + exp.StackTrace);

                    throw exp;
                }
            }
            catch (Exception ex)
            {
                K2PoolLogger.logError("得到连接时出错K2ConnnectionPool.GetConnection(), \r\n "
                    + "currPoolId = " + currPoolId + ", \r\n"
                    + "ex.Messagee = " + ex.Message + "\r\n"
                    + "ex.StackTrace = " + ex.StackTrace);

                if (currPoolId >= 0 && currPoolId < k2ConnWrap.Length)
                {
                    k2ConnWrap[currPoolId].ReTryConn(context);
                    return k2ConnWrap[currPoolId].K2Conn;
                }

            }
            return null;
        }

        /// <summary>
        /// 检查K2连接池是否健康,并修复
        /// </summary>
        private static void CheckK2ConnPool()
        {
            DateTime now = DateTime.Now;

            TimeSpan ts;

            for (int i = 0; i < k2ConnWrap.Length; i++)
            {
                if (k2ConnWrap[i].Status == 0) continue;

                ts = now - k2ConnWrap[i].TimeOfGetting;

                if (ts.TotalMilliseconds > 60000)
                {
                    //释放一个连接占用时间超过一分钟的连接
                    ReleaseConnection(k2ConnWrap[i].K2Conn);

                    logReleaseTimeOutConn(k2ConnWrap[i]);

                    //一次只释放一个连接
                    return;
                }
            }
        }

        private static void logReleaseTimeOutConn(K2ConnWrap wrap)
        {
            string userAccount = "未初始化";
            if (wrap.K2Conn != null)
            {
                if (wrap.K2Conn.User != null)
                {
                    userAccount = wrap.K2Conn.User.Name;
                }
                else
                {
                    userAccount = "K2Conn.User==null";
                }
            }

            string msg = " 释放一个连接占用时间超过一分钟的连接。"
                        + "\r\n | K2服务器：" + wrap.K2ServerName
                        + "\r\n | 初始化时间：" + wrap.TimeOfInit.ToString("HH:mm:ss")
                        + "\r\n | 初始化标识：" + wrap.Initialized.ToString()
                        + "\r\n | 上次所有人：" + userAccount
                        + "\r\n | 得到时间：" + wrap.TimeOfGetting.ToString("HH:mm:ss")
                        + "\r\n | 堆栈：" + wrap.StackOfGetting;

            K2PoolLogger.logError(msg);
        }



        /// <summary>
        /// 释放K2连接池
        /// </summary>
        /// <param name="k2Conn"></param>
        public static void ReleaseConnection(Connection k2Conn)
        {
            //todo:
            bool findPool = false;
            int findPoolId = -1;

            lock (oLock)
            {
                for (int i = 0; i < maxPoolSize; i++)
                {
                    if (k2ConnWrap[i].K2Conn == k2Conn)
                    {
                        findPoolId = i;
                        k2ConnWrap[i].Status = 0;
                        break;
                    }
                }
            }

            if (findPoolId == -1)
            {
                CloseNewK2Conn(k2Conn);
            }

        }

        /// <summary>
        /// 初始化连接池
        /// </summary>
        private static void InitK2ConnPool(ServiceContext context)
        {

            if (IsConstructed) return;

            SetMaxPoolSize();
            k2ConnWrap = new K2ConnWrap[maxPoolSize];

            for (int i = 0; i < k2ConnWrap.Length; i++)
            {
                k2ConnWrap[i] = new K2ConnWrap();
            }

            //string strK2Server = System.Configuration.ConfigurationManager.AppSettings["K2Server"].ToString();
            string strK2Server = context[SettingVariable.ServerName];
            aryServerName = strK2Server.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);


            //初始化结束
            IsConstructed = true;
        }


        /// <summary>
        /// 设置连接池最大值
        /// pcithpx 20110728 [k2Init2]
        /// </summary>
        private static void SetMaxPoolSize()
        {
            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["K2MaxPoolSize"]))
                maxPoolSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["K2MaxPoolSize"].ToString());
            //pcitxxg 2012-11-2 兼容windows服务，用配置设置windows服务的K2最大连接池的值（CGNPC_WindowsServices.exe.config）
            //int curPort = 80;
            //try
            //{
            //    curPort = System.Web.HttpContext.Current.Request.Url.Port;
            //}
            //catch
            //{
            //    try
            //    {
            //        maxPoolSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxPoolSizeWinService"].ToString());
            //    }
            //    catch { }
            //}

            //EnvironmentSitePort sitePort = EnvironmentSitePort.GetInst();

            //if (curPort == sitePort.WebUIPort)
            //{
            //    maxPoolSize = K2ConnectionConfig.MaxPoolSizeWebUI;
            //}
            //else if (curPort == sitePort.WebServicePort)
            //{
            //    maxPoolSize = K2ConnectionConfig.MaxPoolSizeWebService;
            //}
            //else
            //{

            //}
        }

        /// <summary>
        /// 设置K2的参数
        /// pcithpx 20110728 [k2Init2]
        /// </summary>
        /// <param name="k2ConSetup">K2连接参数对象</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        private static void SetK2Param(ConnectionSetup k2ConSetup, string key, string value)
        {
            if (k2ConSetup.ConnectionParameters.ContainsKey(key))
            {
                k2ConSetup.ConnectionParameters[key] = value;
            }
            else
            {
                k2ConSetup.ConnectionParameters.Add(key, value);
            }
        }

        /// <summary>
        /// 连接池无空闲连接，获取新的连接 
        /// </summary>
        /// <returns></returns>
        private static Connection GetNewK2Conn(ServiceContext context)
        {
            InitConfigValue(context);
            Connection k2conn = new Connection();
            ConnectionSetup k2ConSetup = new ConnectionSetup();

            try
            {
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.Host, K2ServerProvider.AvailableServerName);
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.UserID, k2User);
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.Password, k2PassWord);
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.Port, clientPort);
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.Authenticate, "true");
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.SecurityLabelName, label);

                SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.WindowsDomain, domain);
                if (label.ToLower() != "k2")
                    SetK2Param(k2ConSetup, ConnectionSetup.ParamKeys.Integrated, "false");

                k2conn.Open(K2ServerProvider.AvailableServerName, k2ConSetup.ConnectionString);

                lock (oLock)
                {
                    noPoolConnCount++;
                }
            }
            catch (Exception ex)
            {
                K2PoolLogger.logError("得到新连接时出错K2ConnnectionPool.GetNewK2Conn(), \r\n "
                    + "K2ServerProvider.AvailableServerName = " + K2ServerProvider.AvailableServerName + ", \r\n"
                    + "ex.Messagee = " + ex.Message + "\r\n"
                    + "ex.StackTrace = " + ex.StackTrace);
                //2012-2-14 pcitljw
                //得到新连接出错时 重新查找别的主机建立连接
                K2ConnWrap wrap = new K2ConnWrap();
                wrap.K2Conn = k2conn;
                wrap.ReTryConn(context);
                k2conn = wrap.K2Conn;
                //throw;
            }

            return k2conn;
        }

        /// <summary>
        /// 关闭非连接池中的连接
        /// </summary>
        /// <param name="k2Conn"></param>
        private static void CloseNewK2Conn(Connection k2Conn)
        {
            try
            {
                if (k2Conn != null)
                {
                    k2Conn.Close();

                    lock (oLock)
                    {
                        noPoolConnCount--;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (k2Conn != null)
                {
                    k2Conn.Dispose();
                    k2Conn = null;
                }
            }
        }

        private static string getStackInfo()
        {
            StackTrace st = new StackTrace();

            string stackInfo = string.Empty;
            int forlen = 8;
            if (st.FrameCount < 8) forlen = st.FrameCount;

            for (int i = 2; i < forlen; i++)
            {
                StackFrame sf = st.GetFrame(i);

                MethodInfo mi = sf.GetMethod() as MethodInfo;

                if (mi == null || mi.DeclaringType == null) continue;

                stackInfo += mi.DeclaringType.FullName + "." + mi.Name + "--";

            }

            return stackInfo;
        }

    }


    public sealed class K2ConnWrap
    {
        //private static string oServerName = System.Configuration.ConfigurationManager.AppSettings["K2Server"].ToString();
        private static string domain = string.Empty;// System.Configuration.ConfigurationManager.AppSettings["Domain"].ToString();
        private static string k2User = string.Empty;// System.Configuration.ConfigurationManager.AppSettings["K2User"].ToString();
        private static string k2PassWord = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["K2PassWord"].ToString();
        private static string label = string.Empty;
        private static string clientPort = "5252";
        //private static string _timeoutNum = ConfigurationManager.AppSettings["K2ConnTimeOut"].ToString();
        private K2ServerProvider k2Servers = null; // new K2ServerProvider();
        private Connection k2conn;
        public Connection K2Conn
        {
            get
            {
                return k2conn;
            }

            set
            {
                k2conn = value;
            }
        }

        private int status = 0;
        /// <summary>
        /// 该连接是否是在使用中
        /// 0.代表未使用
        /// 1.代表已经使用
        /// </summary>
        public int Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        private bool initialized = false;
        /// <summary>
        /// 该wrap中的连接已经初始化
        /// </summary>
        public bool Initialized
        {
            get
            {
                return initialized;
            }

            set
            {
                initialized = value;
            }
        }

        private DateTime _timeOfGetting = new DateTime(1900, 1, 1);

        /// <summary>
        /// 得到连接的时间
        /// </summary>
        public DateTime TimeOfGetting
        {
            get { return _timeOfGetting; }
            set { _timeOfGetting = value; }
        }

        private string _stackOfGetting = string.Empty;

        /// <summary>
        /// 得到连接的堆栈
        /// </summary>
        public string StackOfGetting
        {
            get { return _stackOfGetting; }
            set { _stackOfGetting = value; }
        }

        private DateTime _timeOfInit = new DateTime(1900, 1, 1);
        /// <summary>
        /// 连接初始化的时间
        /// </summary>
        public DateTime TimeOfInit
        {
            get { return _timeOfInit; }
            set { _timeOfInit = value; }
        }

        private string _k2ServerName = string.Empty;
        /// <summary>
        /// 连接的K2服务器
        /// </summary>
        public string K2ServerName
        {
            get { return _k2ServerName; }
            set { _k2ServerName = value; }
        }



        /// <summary>
        ///  CYK090601
        /// </summary>
        /// <param name="serverName"></param>
        public K2ConnWrap()
        {

            //this._connTime = DateTime.Now;
        }

        public static void InitConfigValue(ServiceContext context)
        {
            domain = context[SettingVariable.WindowDomain];
            k2User = context[SettingVariable.LoginUser];
            k2PassWord = context[SettingVariable.LoginPassword];
            label = context[SettingVariable.SecurityLabelName];
            clientPort = context[SettingVariable.ClientPort];
        }

        public void InitConn(ServiceContext context, string serverName)
        {
            InitConfigValue(context);

            this.k2conn = new Connection();

            ConnectionSetup k2ConSetup = new ConnectionSetup();
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, serverName);
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, k2User);
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, k2PassWord);
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, clientPort);
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.SecurityLabelName, label);
            //k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.TimeOut, _timeoutNum);

            k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, domain);
            if (label.ToLower() != "k2")
                k2ConSetup.ConnectionParameters[ConnectionSetup.ParamKeys.Integrated] = "false";

            this.k2conn.Open(serverName, k2ConSetup.ConnectionString);

            this.TimeOfInit = DateTime.Now;
            this.K2ServerName += serverName + "[" + this.TimeOfInit.ToString("HH:mm:ss") + "],";

        }

        //public K2ConnWrap()
        //{
        //    this.k2conn = new Connection();
        //    try
        //    {
        //        ConnectionSetup k2ConSetup = new ConnectionSetup();
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, k2Servers.getServerName());
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, k2User);
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, k2PassWord);
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, "5252");
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, domain);
        //        k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
        //        //k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.TimeOut, _timeoutNum);

        //        this.k2conn.Open(k2ConSetup);
        //        this.status = 0;
        //    }
        //    catch (Exception e)
        //    {
        //        K2PoolLogger.logError("K2ConnnectionPool.K2ConnWrap():" + e.Message + "\r\n" + e.StackTrace);
        //        throw e;
        //    }
        //    //this._connTime = DateTime.Now;
        //}

        public void ReTryConn(ServiceContext context)
        {
            this.k2Servers = new K2ServerProvider(context);
            try
            {
                this.k2conn.Close();
            }
            catch (Exception)
            { }

            this.k2conn = new Connection();
            try
            {
                ConnectionSetup k2ConSetup = new ConnectionSetup();
                this.k2Servers.findAvailableServer();

                string serverName = this.k2Servers.getServerName();

                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, serverName);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, k2User);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, k2PassWord);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, clientPort);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.SecurityLabelName, label);
                //k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.TimeOut, _timeoutNum);

                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, domain);
                if (label.ToLower() != "k2")
                    k2ConSetup.ConnectionParameters[ConnectionSetup.ParamKeys.Integrated] = "false";

                this.k2conn.Open(serverName, k2ConSetup.ConnectionString);

                this.K2ServerName += "Retry:" + serverName + "[" + DateTime.Now.ToString("HH:mm") + "],";

            }
            catch (Exception e)
            {
                K2PoolLogger.logError("K2ConnnectionPool.ReTryConn():" + e.Message + "\r\n" + e.StackTrace);
                throw e;
            }

            //this.status = 0;
            //this._connTime = DateTime.Now;
        }
    }
    //add by huj 2009-5-11,解决NLB问题,web.config文件中将多个K2Server用逗号隔开
    class K2ServerProvider
    {
        private static string domain = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["Domain"].ToString();
        private static string k2User = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["K2User"].ToString();
        private static string k2PassWord = string.Empty; // System.Configuration.ConfigurationManager.AppSettings["K2PassWord"].ToString();
        private static string label = string.Empty;
        private static string clientPort = "5252";

        private string[] serverNames;
        private int currentServerIndex = 0;
        private static string availableServerName;
        public K2ServerProvider(ServiceContext context)
        {
            K2ServerProvider.InitConfigValue(context);
            //this.serverNames = System.Configuration.ConfigurationManager.AppSettings["K2Server"].ToString().Split(',');
            this.serverNames = context[SettingVariable.ServerName].Split(',');
            K2ServerProvider.availableServerName = this.serverNames[0];
        }

        public static void InitConfigValue(ServiceContext context)
        {
            domain = context[SettingVariable.WindowDomain];
            k2User = context[SettingVariable.LoginUser];
            k2PassWord = context[SettingVariable.LoginPassword];
            label = context[SettingVariable.SecurityLabelName];
            clientPort = context[SettingVariable.ClientPort];
        }

        public static string AvailableServerName
        {
            get
            {
                return availableServerName;
            }
        }
        public string getServerName()
        {
            return this.serverNames[this.currentServerIndex];
        }

        /// <summary>
        /// 查找可用server
        /// </summary>
        public void findAvailableServer()
        {
            for (int i = 0; i < serverNames.Length; i++)
            {
                this.currentServerIndex = i;

                ConnectionSetup k2ConSetup = new ConnectionSetup();
                var serverName = this.serverNames[this.currentServerIndex];
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, serverName);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, k2User);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, k2PassWord);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, clientPort);
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.SecurityLabelName, label);

                k2ConSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, domain);
                if (label.ToLower() != "k2")
                    k2ConSetup.ConnectionParameters[ConnectionSetup.ParamKeys.Integrated] = "false";

                Connection k2conn = new Connection();
                try
                {
                    k2conn.Open(serverName, k2ConSetup.ConnectionString);
                    break;
                }
                catch (Exception e)
                {
                    K2PoolLogger.logError("K2ConnnectionPool.findAvailableServer():" + e.Message + "\r\n" + e.StackTrace);
                }
                finally
                {
                    try
                    {
                        k2conn.Close();
                    }
                    catch (Exception) { }
                }
            }

            K2ServerProvider.availableServerName = this.serverNames[this.currentServerIndex];
        }

    }

    class K2PoolLogger
    {
        public static void logError(string errorMessage)
        {
            //            Database db = DatabaseFactory.CreateDatabase("CGNPCDEVConn");

            //            string strSql =
            //@"INSERT INTO [Platform].[ErrorLog]
            //           ([UserId]
            //           ,[Message])
            //     VALUES
            //           (@UserId
            //           ,@Message)";

            //            DbCommand dbCommand = db.GetSqlStringCommand(strSql.ToString());
            //            db.AddInParameter(dbCommand, "UserId", DbType.String, "");
            //            db.AddInParameter(dbCommand, "Message", DbType.String, errorMessage);

            //            db.ExecuteNonQuery(dbCommand);
        }
    }
}
