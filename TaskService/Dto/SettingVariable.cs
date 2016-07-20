using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tasks.Service.Dto
{
    /// <summary>
    /// This enum define the common variable key
    /// </summary>
    public enum SettingVariable
    {
        /* Common */

        /// <summary>
        /// aZaaS AAPMF Database connection string
        /// </summary>
        //[Required]
        //[DefaultValue("Data Source=.;Initial Catalog=aZaaSAAPMF;Integrated Security=SSPI;")]
        //ConnectionString,


        /* Workflow */
        [DefaultValue("")]
        WorkflowManagementServer,

        [DefaultValue("DLX")]
        WorkflowServer,

        [Required]
        [DefaultValue("DENALLIX")]
        WindowDomain,

        [Required]
        [DefaultValue("DLX")]
        ServerName,

        [Required]
        [DefaultValue(5252)]
        ClientPort,

        [Required]
        [DefaultValue(5555)]
        ServerPort,

        [Required]
        [DefaultValue("Administrator")]
        LoginUser,

        [Required]
        [DefaultValue("K2pass!")]
        LoginPassword,

        [DefaultValue("K2")]
        SecurityLabelName,

        [DefaultValue("Development")]
        Environment,

        [DefaultValue(10000000)]
        ConnectionTimeout,

        /// <summary>
        /// 指定使用K2或者aZaaSBPM,默认使用K2作为流程引擎
        /// </summary>
        [DefaultValue(WFEnigneType.K2)]
        WorkflowEnigneType,
    }

    public enum WFEnigneType
    {
        K2,
        aZaaSBPM
    }
}
