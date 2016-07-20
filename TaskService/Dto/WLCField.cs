using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    /// <summary>
    /// 待办项搜索字段集
    /// Source code is reflected from K2 -> WCField
    /// </summary>
    public enum WLCField
    {
        WorklistItemStatus = 0,
        ProcessName = 1,
        ProcessFullName = 2,
        ProcessFolder = 3,
        ProcessDescription = 4,
        ProcessMetaData = 5,
        ProcessPriority = 6,
        ProcessExpectedDuration = 7,
        ProcessFolio = 8,
        ProcessStartDate = 9,
        ProcessData = 10,
        ProcessXml = 11,
        ActivityName = 12,
        ActivityDescription = 13,
        ActivityMetaData = 14,
        ActivityPriority = 15,
        ActivityExpectedDuration = 16,
        ActivityStartDate = 17,
        ActivityData = 18,
        ActivityXml = 19,
        EventName = 20,
        EventDescription = 21,
        EventMetaData = 22,
        EventPriority = 23,
        EventExpectedDuration = 24,
        EventStartDate = 25,
        ProcessID = 26,
        ProcessStatus = 27,
        RowNumber = 28,
        WorklistItemOwner = 29,
        None = 30,
        SerialNumber = 31,
    }

    /// <summary>
    /// 待办项列表排序规则
    /// Source code is reflected from K2 -> WCField
    /// </summary>
    public enum WLCSortOrder
    {
        Descending,
        Ascending
    }

    /// <summary>
    /// 待办列表Api接口方式
    /// </summary>
    public enum WLApiStyle
    {
        Client,
        Prototype,
    }

    public enum PICField
    {
        Folio,
        StartDate,
        FinishDate,
        ProcessName,
        Status,
        ActivityName,
        Originator
    }

    public enum PICSortOrder
    {
        ASC,
        DESC
    }
}
