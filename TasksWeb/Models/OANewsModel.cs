using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TasksWeb.Models
{
    public class OANewsModel
    {
        public string username { get; set; }
        public string token { get; set; }
        public OANewsDataModel data { get; set; }
    }

    public class OANewsDataModel
    {
        public int siteId { get; set; }
        public string[] toUsers { get; set; }
        public int[] toPartyIds { get; set; }
        public int appId { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public int? status { get; set; }
        public List<Object> elements { get; set; }
        public string detailURL { get; set; }
        public int detailAuth { get; set; }
    }

    public class OANewsDataTextModel
    {
        public string type { get; set; }
        public int? status { get; set; }
        public string content { get; set; }
    }

    public class OANewsDataTextRichModel
    {
        public string type { get; set; }
        public int? status { get; set; }
        public List<OANewsDataTextContentModel> content { get; set; }
    }

    public class OANewsDataTextContentModel
    {
        public int size { get; set; }
        public string color { get; set; }
        public int bold { get; set; }
        public int urlauth { get; set; }
        public string url { get; set; }
        public string text { get; set; }
    }

    public class OANewsDataActionModel
    {
        public string type { get; set; }
        public int? status { get; set; }
        public List<OANewsDataButtonModel> buttons { get; set; }
    }
    public class OANewsDataButtonModel
    {
        public string title { get; set; }
        public string url { get; set; }
    }
}