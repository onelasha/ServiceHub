using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class CallCenterListModel
    {
        public int RowNum { get; set; }
        public int PolcyId {get; set;}
		public string Policy { get; set; }
        public string Pin { get; set; }
        public string Dob { get; set; }
        public string PolicyHolder { get; set; }
        public string Phone { get; set; }
        public string ParentPolicyHolder { get; set; }
        public string Organization { get; set; }
        public string ContractNom { get; set; }
        public int clrfg { get; set; }
    }
}
