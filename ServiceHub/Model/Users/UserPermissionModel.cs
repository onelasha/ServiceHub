using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class UserPermissionModel
    {
        public int rootId { get; set; }
        public int id { get; set; }
        public int pid { get; set; }
        public bool leaf { get; set; }
        public bool expanded { get; set; }
        public bool loaded { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string cls { get; set; }
        public bool hasAccess { get; set; }
        public bool Checked{ get; set; }
        public string reference { get; set; }
        public string url { get; set; }
        public IEnumerable<dynamic> children { get; set; }
    }
}
