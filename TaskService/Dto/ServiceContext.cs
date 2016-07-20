using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    /// <summary>
    /// Service context,it provides basic data configuration.
    /// </summary>
    public class ServiceContext
    {

        /// <summary>
        /// Key-Value configuration variables.
        /// </summary>
        private Dictionary<string, string> contextVariables;

        public static Func<string> ConnectionSetter;

        private static bool hasInit = false;
        private static Dictionary<string, string> globalVariables;
        private const string KEY_AZAASAAPMFDB_LABEL = "aZaaSAAPMFDB";

        public string UserName { get; set; }

        public ServiceAuthType AuthType { get; set; }

        /// <summary>
        /// 租户ID(名称),为多租户的处理，用户登录成功后，要求对其赋值
        /// </summary>
        public string TenantID { get; set; }

        public ServiceContext()
        {
            AuthType = ServiceAuthType.Windows;
            contextVariables = new Dictionary<string, string>();
            Initialize();
        }


        public string this[string key]
        {
            get
            {
                string value = string.Empty;

                //if (key == "ConnectionString" && ServiceContext.ConnectionSetter != null)
                //{
                //    string constr = ConnectionSetter();
                //    if (!string.IsNullOrEmpty(constr))
                //    {
                //        return constr;
                //    }
                //}
                if (!this.contextVariables.TryGetValue(key, out value))
                {
                    if (!globalVariables.TryGetValue(key, out value))
                    {
                        throw new KeyNotFoundException(string.Format("can not found the {0} from the context", key));
                    }
                }
                return value;
            }

            set
            {
                if (!this.contextVariables.ContainsKey(key))
                {
                    this.contextVariables.Add(key, value);
                }
                else
                {
                    this.contextVariables[key] = value;
                }
            }
        }

        public string this[Enum key]
        {
            get
            {
                return this[key.KeyName()];
            }
        }


        public static void Initialize()
        {
            //TODO:Read the configs(web.config,app.config) to global variables dictinary.

            //if (!hasInit)
            //{
            globalVariables = new Dictionary<string, string>();
            ConfigFileVariableInitializer(globalVariables);

            hasInit = true;
            //}

        }

        public static void Initialize(Action<Dictionary<string, string>> initializer)
        {
            //TODO:Read the configs(web.config,app.config) to global variables dictinary.

            if (!hasInit)
            {
                globalVariables = new Dictionary<string, string>();

                if (initializer.NotNull())
                {
                    initializer(globalVariables);

                    hasInit = true;
                }
            }

        }

        public static Action<Dictionary<string, string>> ConfigFileVariableInitializer
        {
            get
            {
                Action<IDictionary<string, string>> initializer = dic =>
                {
                    SettingVariable[] variables = (SettingVariable[])Enum.GetValues(typeof(SettingVariable));
                    foreach (SettingVariable variable in variables)
                    {
                        string variableKey = variable.KeyName();
                        string variableValue = GetVariableValue(variable);

                        if (dic.ContainsKey(variableKey))
                        {
                            dic[variableKey] = variableValue;
                        }
                        else
                        {
                            dic.Add(variableKey, variableValue);
                        }
                    }
                };

                return initializer;
            }
        }

        private static string GetVariableValue(SettingVariable variable)
        {
            string variableKey = variable.KeyName();
            string variableValue = ConfigurationManager.AppSettings[variableKey];

            //Special case for connection string
            //if (variable == SettingVariable.ConnectionString
            //    && variableValue.NullOrEmpty()
            //    && ConfigurationManager.ConnectionStrings[KEY_AZAASAAPMFDB_LABEL].NotNull())
            //{
            //    variableValue = ConfigurationManager.ConnectionStrings[KEY_AZAASAAPMFDB_LABEL].ConnectionString;
            //}

            //if (variable == SettingVariable.ConnectionString
            //    && variableValue.NullOrEmpty())
            //{
            //    variableValue = variable.DefaultValue();
            //}

            if (variableValue.NullOrEmpty())
            {
                if (variable.Required())
                {
                    throw new KeyNotFoundException(string.Format("the variable key [{0}] is not assigned", variableKey));
                }

                variableValue = variable.DefaultValue();
            }

            return variableValue;
        }
    }

    public enum ServiceAuthType
    {
        Windows = 0,
        Form = 1
    }
}

