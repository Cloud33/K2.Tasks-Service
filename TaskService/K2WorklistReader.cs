using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Service.Dto;
using Client = SourceCode.Workflow.Client;
using Runtime = SourceCode.Workflow.Runtime.Worklist;
namespace Tasks.Service
{
    public class K2WorklistReader
    {
        private readonly string _k2ConnectionString;
        private readonly static object _syncLock = new object();
        private Client.Connection conn = null;
        public K2WorklistReader(string k2ConnectionString)
        {
            _k2ConnectionString = k2ConnectionString;
        }

        public void ExecuteAction(string sn, List<DataField> dataFields, string actionName, string comment, bool sync)
        {
            try
            {

                Client.WorklistItem item = conn.OpenWorklistItem(sn);
                foreach (DataField dataField in dataFields)
                {
                    item.ProcessInstance.DataFields[dataField.Name].Value = dataField.Value;
                }
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

        public List<TasksItem> GetTasksItems(string k2User, int? page, int? pageSize, out int totalCount, string procInstID = null, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<string, string> sorting = null)
        {
            k2User = K2User.ApplySecurityLabel(k2User);
            totalCount = 0;

            Client.WorklistCriteria filter = new Client.WorklistCriteria();
            filter.Platform = "ASP";
            filter.Count = (pageSize == null || pageSize <= 0) ? -1 : pageSize.Value;
            filter.StartIndex = (page == null || page <= 0) ? 0 : (page.Value - 1) * filter.Count;

            filter.AddFilterField(Client.WCField.WorklistItemStatus, Client.WCCompare.Equal, Client.WorklistStatus.Available);
            filter.AddFilterField(Client.WCLogical.Or, Client.WCField.WorklistItemStatus, Client.WCCompare.Equal, Client.WorklistStatus.Open);
            filter.AddFilterField(Client.WCLogical.And, Client.WCField.WorklistItemOwner, "Me", Client.WCCompare.Equal, Client.WCWorklistItemOwner.Me); //This will return all the user’s items
            filter.AddFilterField(Client.WCLogical.Or, Client.WCField.WorklistItemOwner, "Other", Client.WCCompare.Equal, Client.WCWorklistItemOwner.Other); //This will return all the user’s shared items

            if (startDate != null)
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessStartDate, Client.WCCompare.GreaterOrEqual, startDate);

            if (endDate != null)
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessStartDate, Client.WCCompare.LessOrEqual, endDate);

            if (!string.IsNullOrEmpty(folio))
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessFolio, Client.WCCompare.Like, string.Format("%{0}%", folio));

            if (!string.IsNullOrEmpty(procInstID))
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessID, Client.WCCompare.Equal, procInstID);

            if (processNames != null && processNames.Any())
            {
                int index = 0;
                foreach (var processName in processNames)
                {
                    index++;
                    if (index == 1)
                        filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessFullName, Client.WCCompare.Equal, processName);
                    else
                        filter.AddFilterField(Client.WCLogical.Or, Client.WCField.ProcessFullName, Client.WCCompare.Equal, processName);
                }

            }

            if (sorting == null || !sorting.Any())
                filter.AddSortField(Client.WCField.EventStartDate, Client.WCSortOrder.Descending);
            else
            {
                foreach (var field in sorting.Keys)
                {
                    filter.AddSortField((Client.WCField)Enum.Parse(typeof(Client.WCField), field), (Client.WCSortOrder)Enum.Parse(typeof(Client.WCSortOrder), sorting[field]));
                }
            }

            var worklit = Runtime.Worklist.OpenWorklist(_k2ConnectionString, k2User, new ArrayList(), filter, false, false, 0, null);
            List<TasksItem> tasks = new List<TasksItem>();
            foreach (Client.WorklistItem item in worklit)
            {
                Actions actions = new Actions();
                foreach (Client.Action act in item.Actions)
                {
                    var action = new ApproveAction();

                    action.Name = act.Name;
                    action.MetaData = act.MetaData;
                    actions.Add(action);
                }
                TasksItem task = new TasksItem()
                {
                    ProcInstID = item.ProcessInstance.ID,
                    ActivityName = item.ActivityInstanceDestination.Name,
                    Destination = K2User.DelApplySecurityLabel(k2User),
                    Folio = item.ProcessInstance.Folio,
                    Originator = item.ProcessInstance.Originator.FQN,
                    //OriginatorDisName = item.ProcessInstance.Originator.DisplayName,
                    SN = item.SerialNumber,
                    StartDate = item.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm"),
                    SharedUser = item.AllocatedUser.Equals(k2User, StringComparison.OrdinalIgnoreCase) ? null : K2User.DelApplySecurityLabel(item.AllocatedUser), //判断是否SharedUser
                    Actions = actions
                };
                tasks.Add(task);
            }
            return tasks;
        }

        public List<WorklistItem> GetWorklistItems(string k2User, int? page, int? pageSize, out int totalCount, string sn = null, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<string, string> sorting = null)
        {
            totalCount = 0;

            Client.WorklistCriteria filter = new Client.WorklistCriteria();
            filter.Platform = "ASP";
            filter.Count = (pageSize == null || pageSize <= 0) ? -1 : pageSize.Value;
            filter.StartIndex = (page == null || page <= 0) ? 0 : (page.Value - 1) * filter.Count;

            filter.AddFilterField(Client.WCField.WorklistItemStatus, Client.WCCompare.Equal, Client.WorklistStatus.Available);
            filter.AddFilterField(Client.WCLogical.Or, Client.WCField.WorklistItemStatus, Client.WCCompare.Equal, Client.WorklistStatus.Open);
            filter.AddFilterField(Client.WCLogical.And, Client.WCField.WorklistItemOwner, "Me", Client.WCCompare.Equal, Client.WCWorklistItemOwner.Me); //This will return all the user’s items
            filter.AddFilterField(Client.WCLogical.Or, Client.WCField.WorklistItemOwner, "Other", Client.WCCompare.Equal, Client.WCWorklistItemOwner.Other); //This will return all the user’s shared items

            if (startDate != null)
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessStartDate, Client.WCCompare.GreaterOrEqual, startDate);

            if (endDate != null)
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessStartDate, Client.WCCompare.LessOrEqual, endDate);

            if (!string.IsNullOrEmpty(folio))
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessFolio, Client.WCCompare.Like, string.Format("%{0}%", folio));

            if (!string.IsNullOrEmpty(sn))
                filter.AddFilterField(Client.WCLogical.And, Client.WCField.SerialNumber, Client.WCCompare.Equal, sn);

            if (processNames != null && processNames.Any())
            {
                int index = 0;
                foreach (var processName in processNames)
                {
                    index++;
                    if (index == 1)
                        filter.AddFilterField(Client.WCLogical.And, Client.WCField.ProcessFullName, Client.WCCompare.Equal, processName);
                    else
                        filter.AddFilterField(Client.WCLogical.Or, Client.WCField.ProcessFullName, Client.WCCompare.Equal, processName);
                }

            }

            if (sorting == null || !sorting.Any())
                filter.AddSortField(Client.WCField.EventStartDate, Client.WCSortOrder.Descending);
            else
            {
                foreach (var field in sorting.Keys)
                {
                    filter.AddSortField((Client.WCField)Enum.Parse(typeof(Client.WCField), field), (Client.WCSortOrder)Enum.Parse(typeof(Client.WCSortOrder), sorting[field]));
                }
            }

            var worklit = Runtime.Worklist.OpenWorklist(_k2ConnectionString, k2User, new ArrayList(), filter, false, false, 0, null);

            totalCount = worklit.TotalCount;
            return this.ToWorklistItems(k2User, worklit, true, false, true);
        }
        private List<WorklistItem> ToWorklistItems(string k2User, Client.Worklist worklist, bool includeInstanceDataFields = false, bool includeActivityDataFields = false, bool includeItemActions = false)
        {
            var wlItems = new List<WorklistItem>();

            foreach (Client.WorklistItem item in worklist)
            {
                wlItems.Add(ToWorklistItem(k2User, item, includeInstanceDataFields, includeActivityDataFields, includeItemActions));
            }

            return wlItems;
        }
        private WorklistItem ToWorklistItem(string k2User, Client.WorklistItem item, bool includeInstanceDataFields = false, bool includeActivityDataFields = false, bool includeItemActions = false)
        {
            if (item == null) return null;

            var wlItem = new Tasks.Service.Dto.WorklistItem();

            wlItem.ID = item.ID;
            wlItem.ProcInstID = item.ProcessInstance.ID;
            wlItem.ActInstDestID = item.ActivityInstanceDestination.ID;
            wlItem.ActInstID = item.ActivityInstanceDestination.ActInstID;
            wlItem.ActID = item.ActivityInstanceDestination.ActID;
            wlItem.EventID = item.EventInstance.ID;
            wlItem.Destination = K2User.DelApplySecurityLabel(item.AllocatedUser);
            wlItem.AssignedDate = item.ActivityInstanceDestination.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.StartDate = item.EventInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.Status = (Tasks.Service.Dto.WorklistStatus)(int)item.Status;
            wlItem.ActivityName = item.ActivityInstanceDestination.Name;
            wlItem.ActivityDispName = item.ActivityInstanceDestination.Name;
            wlItem.FullName = item.ProcessInstance.FullName;
            wlItem.ProcDispName = item.ProcessInstance.Name;
            wlItem.Folio = item.ProcessInstance.Folio;
            wlItem.ProcInstStatus = (ProcInstStatus)(int)item.ProcessInstance.Status1;
            wlItem.ProcStartDate = item.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.SN = item.SerialNumber;
            wlItem.Data = item.Data;
            wlItem.Originator = K2User.DelApplySecurityLabel(item.ProcessInstance.Originator.FQN);

            if (!item.AllocatedUser.Equals(k2User, StringComparison.OrdinalIgnoreCase))
            {
                wlItem.Data += "&SharedUser=" + item.AllocatedUser;
            }

            if (item.ProcessInstance != null)
            {
                wlItem.ProcessInstance = ToProcessInstance(item.ProcessInstance, includeInstanceDataFields);
            }

            if (includeActivityDataFields)
            {
                var actDataFields = new Dictionary<string, object>();

                foreach (Client.DataField field in item.ActivityInstanceDestination.DataFields)
                {
                    try
                    {
                        if (actDataFields.ContainsKey(field.Name))
                            actDataFields[field.Name] = field.Value;
                        else
                            actDataFields.Add(field.Name, field.Value);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
                wlItem.ActivityDataFields = actDataFields;
            }

            if (includeItemActions)
            {
                foreach (Client.Action act in item.Actions)
                {
                    var action = new ApproveAction();

                    action.Name = act.Name;
                    action.MetaData = act.MetaData;
                    //action.WorklistItem = wlItem;
                    wlItem.Actions.Add(action);
                }
            }

            return wlItem;
        }

        private Tasks.Service.Dto.ProcessInstance ToProcessInstance(Client.ProcessInstance inst, bool includeInstanceDataFields = false)
        {
            if (inst == null) return null;

            var procInst = new Tasks.Service.Dto.ProcessInstance();

            procInst.ID = inst.ID;
            procInst.FullName = inst.FullName;
            procInst.Folio = inst.Folio;
            procInst.Originator = K2User.DelApplySecurityLabel(inst.Originator.FQN);
            procInst.Priority = inst.Priority;
            procInst.ExpectedDuration = inst.ExpectedDuration;
            procInst.StartDate = inst.StartDate.ToString("yyyy-MM-dd HH:mm");

            if (includeInstanceDataFields)
            {
                if (inst.DataFields != null)
                {
                    Dictionary<string, Tasks.Service.Dto.DataField> dicFields = new Dictionary<string, Tasks.Service.Dto.DataField>();
                    foreach (Client.DataField field1 in inst.DataFields)
                    {
                        Tasks.Service.Dto.DataField field = new Tasks.Service.Dto.DataField();

                        try
                        {
                            field.Name = field1.Name;
                            field.Value = field1.Value;
                            dicFields.Add(field.Name, field);
                        }
                        catch (Exception)
                        {

                            continue;
                        }

                    }
                    procInst.DataFields = dicFields;
                }

                if (inst.XmlFields != null)
                {
                    Dictionary<string, XmlField> dicXmlFields = new Dictionary<string, XmlField>();
                    foreach (Client.XmlField field1 in inst.XmlFields)
                    {
                        XmlField field = new XmlField();
                        try
                        {
                            field.Name = field1.Name;
                            field.Value = field1.Value;
                            field.Category = field1.Category;
                            field.Hidden = field1.Hidden;
                            field.MetaData = field1.MetaData;
                            field.Schema = field1.Schema;
                            field.Xsl = field1.Xsl;
                            dicXmlFields.Add(field.Name, field);
                        }
                        catch (Exception)
                        {
                            continue;
                        }


                    }
                    procInst.XmlFields = dicXmlFields;
                }
            }

            return procInst;
        }

    }
}
