using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class K2WorkflowUtility
    {

        public static void ReleaseConnectionFromPool(Connection conn)
        {
            try
            {
                K2ConnectionPool.ReleaseConnection(conn);
            }
            catch (Exception ex)
            {
                if (conn != null)
                    CloseConnection(conn);
            }
        }
        public static Connection GetConnection(ServiceContext context)
        {
            Connection conn = new Connection();

            ConnectionSetup conSetup = new ConnectionSetup();

            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, context[SettingVariable.ServerName]);
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, context[SettingVariable.LoginUser]);
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, context[SettingVariable.LoginPassword]);
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, context[SettingVariable.ClientPort]);
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, context[SettingVariable.WindowDomain]);
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.TimeOut, context[SettingVariable.ConnectionTimeout]);

            var label = context[SettingVariable.SecurityLabelName];
            conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.SecurityLabelName, label);
            if (label.ToLower() != "k2")
            {
                conSetup.ConnectionParameters[ConnectionSetup.ParamKeys.Integrated] = "false";
            }

            conn.Open(context[SettingVariable.ServerName], conSetup.ConnectionString);

            return conn;
        }
        public static Connection GetConnectionFromPool(ServiceContext context)
        {
            Connection conn = null;
            try
            {
                conn = K2ConnectionPool.GetConnection(context);
                if (conn != null && !string.IsNullOrEmpty(context.UserName) && conn.User.Name.ToLower() != context.UserName.ToLower())
                    conn.ImpersonateUser(context.UserName);
            }
            catch (Exception ex)
            {
                if (conn == null)
                    conn = GetConnection(context);
            }
            return conn;
        }
        public static void CloseConnection(Connection conn)
        {
            bool isRelease = false;
            try
            {
                if (conn != null)
                {
                    conn.Close();
                    isRelease = true;
                }
            }
            finally
            {
                if (conn != null && !isRelease)
                {
                    conn.Dispose();
                    conn = null;
                }
            }
        }

    }
}
