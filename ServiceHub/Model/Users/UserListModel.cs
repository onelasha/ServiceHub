using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class UserListModel
    {
        public int RowNum { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserDescription { get; set; }
        public string UserCode { get; set; }
        public string Hostname { get; set; }
        public string LastLogginDate { get; set; }
        public bool IsMed { get; set; }
        public bool IsSales { get; set; }
        public bool IsBlocked { get; set; }
        public int clrfg { get; set; }
    }
}
