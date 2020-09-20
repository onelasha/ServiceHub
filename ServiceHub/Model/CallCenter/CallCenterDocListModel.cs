using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class CallCenterDocListModel
    {
        public int RowNum { get; set; }
        public int SaxeobaId { get; set; }
        public int DocId { get; set; }
        public int DocName { get; set; }
        public int PolcyId {get; set;}
		public string Policy { get; set; }
        public string Pin { get; set; }
        public string Dob { get; set; }
        public string PolicyHolder { get; set; }
        public string Phone { get; set; }
        public string ParentPolicyHolder { get; set; }
        public string Organization { get; set; }
        public string ContractNom { get; set; }
        public string SaxeobaName { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public int clrfg { get; set; }
        public string iconCls { get; set; }
    }
}
