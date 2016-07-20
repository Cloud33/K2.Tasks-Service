using SourceCode.Workflow.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public static class K2User
    {
        public const string DEFAULT_K2SECURITY_LABEL = "K2";
        public const string DEFAULT_K2SQLSECURITY_LABEL = "K2SQL";
        private static string SecurityLabelName = ConfigurationManager.AppSettings["SecurityLabelName"];
        private static string WindowDomain = ConfigurationManager.AppSettings["WindowDomain"];
        public static Func<string, string> UserSecurityLabelResolver { get; set; }
        public static Func<string> K2SecurityLabelNameProvider { get; set; }
        public static Func<string> K2SQLSecurityLabelNameProvider { get; set; }


        public static string K2SecurityLabel
        {
            get
            {
                return K2SecurityLabelNameProvider == null ? DEFAULT_K2SECURITY_LABEL : K2SecurityLabelNameProvider();
            }
        }

        public static string K2SQLSecurityLabel
        {
            get
            {
                return K2SQLSecurityLabelNameProvider == null ? DEFAULT_K2SQLSECURITY_LABEL : K2SQLSecurityLabelNameProvider();
            }
        }

        //public static bool Test(string userName, string password)
        //{
        //    if (userName.Contains(":"))
        //        userName = userName.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1];

        //    var integratedSecurity = userName.Contains(@"\");
        //    var currentSecurityLabel = integratedSecurity ? K2SecurityLabel : K2SQLSecurityLabel;

        //    var userNameParts = userName.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

        //    userName = integratedSecurity ? userNameParts[1] : userName;
        //    var windowsDomain = !integratedSecurity ? string.Empty : userNameParts[0];

        //    return Test(userName, password, currentSecurityLabel, integratedSecurity, windowsDomain);
        //}

        //public static bool Test(string userName, string password, string securityLabel = "K2", bool integratedSecurity = true, string windowsDomain = "")
        //{
        //    var isValid = true;
        //    var conn = new Connection();
        //    var context = new ServiceContext();

        //    var conSetup = new ConnectionSetup();

        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Host, context[SettingVariable.ServerName]);
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.UserID, userName);
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Password, password);
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Port, context[SettingVariable.ClientPort]);
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.Authenticate, "true");

        //    var currentsDomain = string.IsNullOrEmpty(windowsDomain) ? context[SettingVariable.WindowDomain] : windowsDomain;
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.WindowsDomain, currentsDomain);

        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.IsPrimaryLogin, "true");
        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.TimeOut, context[SettingVariable.ConnectionTimeout]);

        //    conSetup.ConnectionParameters.Add(ConnectionSetup.ParamKeys.SecurityLabelName, securityLabel);
        //    conSetup.ConnectionParameters[ConnectionSetup.ParamKeys.Integrated] = integratedSecurity ? "true" : "false";

        //    try
        //    {
        //        conn.Open(context[SettingVariable.ServerName], conSetup.ConnectionString);
        //    }
        //    catch (Exception)
        //    {
        //        isValid = false;
        //    }
        //    finally
        //    {
        //        K2WorkflowUtility.CloseConnection(conn);
        //    }

        //    return isValid;
        //}

        public static string ApplySecurityLabel(string userName)
        {
            //if (UserSecurityLabelResolver != null)
            //    return UserSecurityLabelResolver(userName);
            return DefaultUserSecurityLabelResolver(userName);
        }


        private static string DefaultUserSecurityLabelResolver(string userName)
        {
            if (userName.Contains(":")) //If user already appended label, we'll use currenty label.
                return userName;

            //else if user is domian user we'll append "K2" label or "K2SQL" for k2sql user.
            //return string.Format("{0}:{1}", userName.Contains(@"\") ? K2SecurityLabel : K2SQLSecurityLabel, userName);
            if (SecurityLabelName == "K2")
            {
                return string.Format("{0}:{1}", SecurityLabelName, userName.Contains(@"\") ? userName : WindowDomain + "\\" + userName);
            }
            else
            {
                return string.Format("{0}:{1}", SecurityLabelName, userName);
            }
        }

        public static string DelApplySecurityLabel(string userName)
        {
            return userName.Replace("K2:", "").Replace("K2SQL:", "");
        }


    }
}
