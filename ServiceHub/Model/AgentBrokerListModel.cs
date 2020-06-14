using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class AgentBrokerListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsBroker { get; set; }
        public bool IsIndMetsarme { get; set; }
        public int EntityType { get; set; }
        public int clrfg { get; set; }
    }
}
