using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Service.Dto;

namespace Tasks.Service
{
    public class WorkflowClientService
    {
        private string K2Server = ConfigurationManager.AppSettings["ServerName"];
        private string K2AdminConnection = ConfigurationManager.AppSettings["K2AdminConnection"];

        private SourceCode.Workflow.Client.Connection conn = null;
        /// <summary>
        /// 任务列表 手机使用数据
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="procInstID"></param>
        /// <param name="folio"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processNames"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public IEnumerable<TasksItem> GetTasksItems(string userName, int? page, int? pageSize, out int totalCount, string procInstID = null, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<string, string> sorting = null)
        {
            return GetTasksItemsByPrototypeAPI(userName, page, pageSize, out totalCount, procInstID, folio, startDate, endDate, processNames, sorting);
        }

        public List<WorklistItem> GetWorklistItem(string userName, int? page, int? pageSize, out int totalCount, string sn = null, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<string, string> sorting = null)
        {
            var k2User = K2User.ApplySecurityLabel(userName);
            var worklistReader = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);

            return worklistReader.GetWorklistItems(k2User, page, pageSize, out totalCount, sn, folio, startDate, endDate, processNames, sorting);
        }
        private IEnumerable<TasksItem> GetTasksItemsByPrototypeAPI(string userName, int? page, int? pageSize, out int totalCount, string procInstID = null, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<string, string> sorting = null)
        {
            var k2User = K2User.ApplySecurityLabel(userName);
            var worklistReader = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);

            var sort = new Dictionary<string, string>();

            if (sorting != null && sorting.Any())
            {
                foreach (var field in sorting.Keys)
                {
                    sort.Add(Enum.GetName(typeof(WLCField), field), Enum.GetName(typeof(WLCSortOrder), sorting[field]));
                }
            }

            return worklistReader.GetTasksItems(k2User, page, pageSize, out totalCount, procInstID, folio, startDate, endDate, processNames, sort);
        }

        public void ExecuteAction(string userName, string sn, string actionName, bool sync)
        {
            try
            {
                GetK2OpenConnection(userName);
                SourceCode.Workflow.Client.WorklistItem item = conn.OpenWorklistItem(sn);
                item.ActivityInstanceDestination.DataFields["_TASK_ACTIONTAKER"].Value = userName;
                item.ActivityInstanceDestination.DataFields["_TASK_ACTIONCOMMENT"].Value = actionName;
                item.Actions[actionName].Execute(sync);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public void ExecuteAction(string userName, string sn, string actionName, bool sync, string sharedUser)
        {
            try
            {
                ;
                GetK2OpenConnection(K2User.ApplySecurityLabel(userName));
                SourceCode.Workflow.Client.WorklistItem item = null;
                if (!string.IsNullOrEmpty(sharedUser))
                    item = conn.OpenSharedWorklistItem(K2User.ApplySecurityLabel(sharedUser), string.Empty, sn, "ASP", false);
                else
                    item = conn.OpenWorklistItem(sn, "ASP", false);
                item.ActivityInstanceDestination.DataFields["_TASK_ACTIONTAKER"].Value = userName;
                item.ActivityInstanceDestination.DataFields["_TASK_ACTIONCOMMENT"].Value = actionName;
                item.Actions[actionName].Execute(sync);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }


        public WorklistItem OpenWorklistItem(string userName, string sn, string shareUser)
        {
            try
            {
                ServiceContext context = new ServiceContext();
                context.UserName = K2User.ApplySecurityLabel(userName);
                GetK2OpenConnection(context.UserName);
                SourceCode.Workflow.Client.WorklistItem clientItem;
                if (shareUser == "null")
                {
                    shareUser = "";
                }
                if (!string.IsNullOrEmpty(shareUser))
                {
                    clientItem = conn.OpenSharedWorklistItem(shareUser, null, sn);
                }
                else
                {
                    clientItem = conn.OpenWorklistItem(sn);
                }

                WorklistItem item = ObjectConverter.ConvertToWFWorklistItem(context, clientItem);
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public WorklistItem OpenWorklistItem(string userName, string sn, PlatformType platform, bool allocated)
        {
            try
            {
                ServiceContext context = new ServiceContext();
                context.UserName = K2User.ApplySecurityLabel(userName);
                GetK2OpenConnection(context.UserName);
                SourceCode.Workflow.Client.WorklistItem clientItem = conn.OpenWorklistItem(sn);
                WorklistItem item = ObjectConverter.ConvertToWFWorklistItem(context, clientItem);
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public WorklistItem OpenWorklistItem(string userName, string shareUser, string sn, PlatformType platform, bool allocated)
        {
            try
            {
                ServiceContext context = new ServiceContext();
                context.UserName = K2User.ApplySecurityLabel(shareUser);
                GetK2OpenConnection(context.UserName);
                SourceCode.Workflow.Client.WorklistItem clientItem = null;
                if (!string.IsNullOrEmpty(shareUser))
                    clientItem = conn.OpenSharedWorklistItem(shareUser, string.Empty, sn, platform.ToString(), allocated);
                else
                    clientItem = conn.OpenWorklistItem(sn, platform.ToString(), allocated);
                WorklistItem item = ObjectConverter.ConvertToWFWorklistItem(context, clientItem);
                return item;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }


        private void GetK2OpenConnection(string userAccount)
        {
            conn = new SourceCode.Workflow.Client.Connection();
            conn.Open(K2Server, this.K2AdminConnection);
            conn.ImpersonateUser(userAccount);

        }
    }

}
