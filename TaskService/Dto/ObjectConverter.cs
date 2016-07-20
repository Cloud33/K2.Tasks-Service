using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Service.Dto;
using Client = SourceCode.Workflow.Client;
using Mgt = SourceCode.Workflow.Management;


namespace Tasks.Service.Dto
{
    public class ObjectConverter
    {
        public static ProcessInstance ConvertToWFProcessInstance(Client.ProcessInstance origObj, bool loadDataFields)
        {
            if (origObj == null)
            {
                return null;
            }
            ProcessInstance procInst = new ProcessInstance();
            procInst.ID = origObj.ID;
            //procInst.ProcID = origObj.ProcID;
            //procInst.ProcSetID = origObj.ProcSetID;
            procInst.FullName = origObj.FullName;
            //procInst.DisplayName = origObj.DisplayName;
            //procInst.FlowNumber = origObj.FlowNumber;
            procInst.Folio = origObj.Folio;
            procInst.Originator = origObj.Originator.Name;
            ///procInst.Status = (ProcInstStatus)(int)origObj.Status1;
            procInst.Priority = origObj.Priority;
            procInst.ExpectedDuration = origObj.ExpectedDuration;
            procInst.StartDate = origObj.StartDate.ToString("yyyy-MM-dd HH:mm");
            if (loadDataFields)
            {
                if (origObj.DataFields != null)
                {
                    Dictionary<string, DataField> dicFields = new Dictionary<string, DataField>();
                    foreach (Client.DataField field1 in origObj.DataFields)
                    {
                        DataField field = new DataField();
                        field.Name = field1.Name;
                        //field.Type = origObj.DataFields[fieldName].Type.ToString();
                        field.Value = field1.Value;
                        //field.Category = origObj.DataFields[fieldName].Category;
                        dicFields.Add(field.Name, field);
                    }
                    procInst.DataFields = dicFields;
                }
                if (origObj.XmlFields != null)
                {
                    Dictionary<string, XmlField> dicXmlFields = new Dictionary<string, XmlField>();
                    foreach (Client.XmlField field1 in origObj.XmlFields)
                    {
                        XmlField field = new XmlField();
                        field.Name = field1.Name;
                        field.Value = field1.Value;
                        field.Category = field1.Category;
                        field.Hidden = field1.Hidden;
                        field.MetaData = field1.MetaData;
                        field.Schema = field1.Schema;
                        field.Xsl = field1.Xsl;
                        dicXmlFields.Add(field.Name, field);
                    }
                    procInst.XmlFields = dicXmlFields;
                }
            }
            return procInst;
        }


        public static WorklistItem ConvertToWFWorklistItem(Client.WorklistItem origObj)
        {
            if (origObj == null)
            {
                return null;
            }
            WorklistItem wlItem = new WorklistItem();
            wlItem.ID = origObj.ID;
            wlItem.ProcInstID = origObj.ProcessInstance.ID;
            wlItem.ActInstDestID = origObj.ActivityInstanceDestination.ID;
            wlItem.ActInstID = origObj.ActivityInstanceDestination.ActInstID;
            //wlItem.ProcID = origObj.ProcID;
            wlItem.ActID = origObj.ActivityInstanceDestination.ActID;
            wlItem.EventID = origObj.EventInstance.ID;
            wlItem.Destination = origObj.AllocatedUser;
            wlItem.AssignedDate = origObj.ActivityInstanceDestination.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.StartDate = origObj.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            //wlItem.FinishDate = origObj.FinishDate;
            wlItem.Status = (WorklistStatus)(int)origObj.Status;
            //wlItem.ProfileID = origObj.ProfileID;
            //wlItem.TenantID = origObj.TenantID;
            wlItem.ActivityName = origObj.ActivityInstanceDestination.Name;
            wlItem.ActivityDispName = origObj.ActivityInstanceDestination.Name;
            wlItem.FullName = origObj.ProcessInstance.FullName;
            wlItem.ProcDispName = origObj.ProcessInstance.Name;
            //wlItem.Originator = origObj.;
            wlItem.Folio = origObj.ProcessInstance.Folio;
            //wlItem.FlowNumber = origObj.ProcessInstance.;
            wlItem.ProcInstStatus = (ProcInstStatus)(int)origObj.ProcessInstance.Status1;
            wlItem.ProcStartDate = origObj.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.SN = origObj.SerialNumber;
            wlItem.Data = origObj.Data;

            if (origObj.ProcessInstance != null)
            {
                wlItem.ProcessInstance = ConvertToWFProcessInstance(origObj.ProcessInstance, true);
            }

            foreach (Client.Action origAction in origObj.Actions)
            {
                ApproveAction action = new ApproveAction();
                action.Name = origAction.Name;
                action.MetaData = origAction.MetaData;
                //action.WorklistItem = wlItem;
                wlItem.Actions.Add(action);
            }
            return wlItem;
        }

        public static WorklistItem ConvertToWFWorklistItem(ServiceContext context, Client.WorklistItem origObj)
        {
            if (origObj == null)
            {
                return null;
            }
            WorklistItem wlItem = new WorklistItem();
            wlItem.ID = origObj.ID;
            wlItem.ProcInstID = origObj.ProcessInstance.ID;
            wlItem.ActInstDestID = origObj.ActivityInstanceDestination.ID;
            wlItem.ActInstID = origObj.ActivityInstanceDestination.ActInstID;
            //wlItem.ProcID = origObj.ProcID;
            wlItem.ActID = origObj.ActivityInstanceDestination.ActID;
            wlItem.EventID = origObj.EventInstance.ID;
            wlItem.Destination = origObj.AllocatedUser;
            wlItem.AssignedDate = origObj.ActivityInstanceDestination.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.StartDate = origObj.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            //wlItem.FinishDate = origObj.FinishDate;
            wlItem.Status = (WorklistStatus)(int)origObj.Status;
            //wlItem.ProfileID = origObj.ProfileID;
            //wlItem.TenantID = origObj.TenantID;
            wlItem.ActivityName = origObj.ActivityInstanceDestination.Name;
            wlItem.ActivityDispName = origObj.ActivityInstanceDestination.Name;
            wlItem.FullName = origObj.ProcessInstance.FullName;
            wlItem.ProcDispName = origObj.ProcessInstance.Name;
            //Added By:BingYi 2014-07-17 Fixed:The Originator should be included in worklist item.
            wlItem.Originator = origObj.ProcessInstance.Originator.Name;
            wlItem.Folio = origObj.ProcessInstance.Folio;
            //wlItem.FlowNumber = origObj.ProcessInstance.;
            wlItem.ProcInstStatus = (ProcInstStatus)(int)origObj.ProcessInstance.Status1;
            wlItem.ProcStartDate = origObj.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.SN = origObj.SerialNumber;
            wlItem.Data = origObj.Data;
            if (origObj.AllocatedUser.ToLower() != (K2User.ApplySecurityLabel(context.UserName)).ToLower())
            {
                wlItem.Data += "&SharedUser=" + origObj.AllocatedUser;
            }

            if (origObj.ProcessInstance != null)
            {
                wlItem.ProcessInstance = ConvertToWFProcessInstance(origObj.ProcessInstance, true);
            }

            var activityDataFields = new Dictionary<string, object>();
            foreach (Client.DataField item in origObj.ActivityInstanceDestination.DataFields)
            {
                if (activityDataFields.ContainsKey(item.Name))
                    activityDataFields[item.Name] = item.Value;
                else
                    activityDataFields.Add(item.Name, item.Value);
            }
            wlItem.ActivityDataFields = activityDataFields;

            foreach (Client.Action origAction in origObj.Actions)
            {
                ApproveAction action = new ApproveAction();
                action.Name = origAction.Name;
                action.MetaData = origAction.MetaData;
                //action.WorklistItem = wlItem;
                wlItem.Actions.Add(action);
            }
            return wlItem;
        }


        public static ProcessInstance ConvertToWFProcessInstance(Mgt.ProcessInstance origObj, bool loadDataFields)
        {
            if (origObj == null)
            {
                return null;
            }
            ProcessInstance procInst = new ProcessInstance();
            procInst.ID = origObj.ID;
            procInst.ProcID = origObj.ProcID;
            procInst.ProcSetID = origObj.ProcSetID;
            procInst.FullName = origObj.ProcSetFullName;
            if (origObj.Process != null)
            {
                procInst.FullName = origObj.Process.FullName;
            }

            procInst.Folio = origObj.Folio;
            procInst.Originator = origObj.Originator;
            procInst.Status = (ProcInstStatus)Enum.Parse(typeof(ProcInstStatus), origObj.Status);
            procInst.Priority = origObj.Priority;
            procInst.ExpectedDuration = origObj.ExpectedDuration;
            procInst.StartDate = origObj.StartDate.ToString("yyyy-MM-dd HH:mm");
            procInst.FinishDate = origObj.FinishDate.ToString("yyyy-MM-dd HH:mm");
            if (loadDataFields && origObj.Process != null && origObj.Process.DataFields != null)
            {
                Dictionary<string, DataField> dicFields = new Dictionary<string, DataField>();
                foreach (Client.DataField field1 in origObj.Process.DataFields)
                {
                    DataField field = new DataField();
                    field.Name = field1.Name;
                    //field.Type = origObj.DataFields[fieldName].Type.ToString();
                    field.Value = field1.Value;
                    //field.Category = origObj.DataFields[fieldName].Category;
                    dicFields.Add(field.Name, field);
                }
                procInst.DataFields = dicFields;
            }
            return procInst;
        }

        public static WorklistItem ConvertToWFWorklistItem(Mgt.WorklistItem origObj)
        {
            if (origObj == null)
            {
                return null;
            }
            WorklistItem wlItem = new WorklistItem();
            wlItem.ID = origObj.ID;

            wlItem.ProcInstID = origObj.ProcInstID;
            wlItem.ActInstDestID = origObj.ActInstDestID;
            wlItem.ActInstID = origObj.ActInstID;
            wlItem.ActID = origObj.ActID;
            wlItem.EventID = origObj.EventID;
            wlItem.Destination = origObj.Destination;
            wlItem.StartDate = origObj.StartDate.ToString("yyyy-MM-dd HH:mm");
            wlItem.Status = (WorklistStatus)(int)origObj.Status;
            wlItem.ActivityName = origObj.ActivityName;
            wlItem.ActivityDispName = origObj.ActivityName;
            wlItem.FullName = origObj.ProcName;
            wlItem.ProcDispName = origObj.ProcName;
            wlItem.Folio = origObj.Folio;
            wlItem.ProcInstStatus = (ProcInstStatus)(int)origObj.ProcessInstanceStatus;

            return wlItem;
        }

        public static DataField ConvertToWFDataField(Client.DataField origObj)
        {
            if (origObj == null)
            {
                return null;
            }
            DataField field = new DataField();
            field.Name = origObj.Name;
            field.Value = origObj.Value;
            return field;
        }

        //public static ErrorLog ConvertToErrorLog(Mgt.ErrorLog origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    ErrorLog log = new ErrorLog();
        //    log.ID = origObj.ID;
        //    log.FullName = origObj.ProcessName;
        //    log.ProcDispName = origObj.ProcessName;
        //    log.ProcInstID = origObj.ProcInstID;
        //    log.ItemName = origObj.ErrorItemName;
        //    log.ObjectID = origObj.ObjectID;
        //    log.LogDate = origObj.ErrorDate;
        //    log.StackTrace = origObj.StackTrace;
        //    log.Message = origObj.Description;

        //    return log;
        //}

        //public static ProcessVersion ConvertToProcessVersion(Mgt.Process origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    ProcessVersion ver = new ProcessVersion();
        //    ver.ID = origObj.ProcID;
        //    ver.ProcSetID = origObj.ProcSetID;
        //    ver.Guid = origObj.ProcGuid;
        //    ver.ChangeDate = origObj.VersionDate;
        //    ver.Priority = origObj.Priority;
        //    ver.MetaData = origObj.MetaData;
        //    ver.DefaultVerID = origObj.DefaultVersion ? origObj.VersionNumber : 0;
        //    ver.Version = origObj.VersionNumber;
        //    ver.ExpectedDuration = origObj.ExpectedDuration;

        //    return ver;
        //}

        //public static Activity ConvertToActivity(Mgt.Activity origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    Activity act = new Activity();
        //    act.ID = origObj.ID;
        //    act.ProcID = origObj.ProcID;
        //    act.Name = origObj.Name;
        //    return act;
        //}

        //public static StringTable ConvertToStringTable(Mgt.StringTableEntry origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    StringTable st = new StringTable();
        //    st.Name = origObj.Name;
        //    st.Value = origObj.Value;
        //    return st;
        //}

        //public static ProcessPermission ConvertToProcessPermission(Mgt.ProcSetPermissions origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    ProcessPermission perm = new ProcessPermission();
        //    perm.AuthType = string.IsNullOrEmpty(origObj.UserName) ? (string.IsNullOrEmpty(origObj.RoleName) ? "Group" : "Role") : "User";
        //    perm.ProcSetID = origObj.ProcSetID;
        //    perm.FullName = origObj.ProcessFullName;
        //    perm.DisplayName = origObj.ProcessName;
        //    perm.AuthName = perm.AuthType == "User" ? origObj.UserName : (perm.AuthType == "Role" ? origObj.RoleName : origObj.GroupName);
        //    perm.Admin = origObj.Admin;
        //    perm.Start = origObj.Start;
        //    perm.View = origObj.View;
        //    perm.ViewPart = origObj.ViewPart;
        //    perm.ServerEvent = origObj.ServerEvent;
        //    //perm.AuthID = origObj.AuthID;
        //    //perm.Permission = origObj.Permission;
        //    //perm.TenantID = origObj.TenantID;
        //    return perm;
        //}

        //public static Mgt.ProcSetPermissions ConvertToMgtProcessPermission(ProcessPermission origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    Mgt.ProcSetPermissions perm = new Mgt.ProcSetPermissions();
        //    perm.ProcSetID = origObj.ProcSetID;
        //    perm.ProcessFullName = origObj.FullName;
        //    perm.ProcessName = origObj.DisplayName;
        //    perm.UserName = origObj.AuthType == "User" ? origObj.AuthName : string.Empty;
        //    perm.RoleName = origObj.AuthType == "Role" ? origObj.AuthName : string.Empty;
        //    perm.GroupName = origObj.AuthType == "Group" ? origObj.AuthName : string.Empty;
        //    perm.Admin = origObj.Admin;
        //    perm.Start = origObj.Start;
        //    perm.View = origObj.View;
        //    perm.ViewPart = origObj.ViewPart;
        //    perm.ServerEvent = origObj.ServerEvent;
        //    //perm.AuthID = origObj.AuthID;
        //    //perm.Permission = origObj.Permission;
        //    //perm.TenantID = origObj.TenantID;
        //    return perm;
        //}

        //public static ServerPermission ConvertToServerPermission(Mgt.AdminPermission origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    ServerPermission perm = new ServerPermission();
        //    perm.UserName = origObj.UserName;
        //    perm.Admin = origObj.Admin;
        //    perm.Export = origObj.Export;
        //    perm.Impersonate = origObj.CanImpersonate;

        //    return perm;
        //}

        //public static Mgt.AdminPermission ConvertToMgtServerPermission(ServerPermission origObj)
        //{
        //    if (origObj == null)
        //    {
        //        return null;
        //    }
        //    Mgt.AdminPermission perm = new Mgt.AdminPermission();
        //    perm.UserName = origObj.UserName;
        //    perm.Admin = origObj.Admin;
        //    perm.Export = origObj.Export;
        //    perm.CanImpersonate = origObj.Impersonate;
        //    return perm;
        //}

        //public static ProcessSet ConvertToProcessSet(Mgt.ProcessSet origObj)
        //{
        //    if (origObj == null)
        //        return null;
        //    ProcessSet set = new ProcessSet();
        //    set.ID = origObj.ProcSetID;
        //    set.FullName = origObj.FullName;
        //    set.DisplayName = origObj.FullName;
        //    set.Descr = origObj.Description;
        //    set.Name = origObj.Name;
        //    set.Folder = origObj.Folder;
        //    return set;
        //}
    }
}
