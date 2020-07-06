using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//reestrContractList
namespace ServiceHub.Model
{
    public class ContractListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
		public int RecState { get; set; }
		public int DamzgveviId { get; set; }
		public string ContractName { get; set; }
		public string DamzgveviDescription { get; set; }
		public string DateStart { get; set; }
		public string DateEnd { get; set; }
		public string DateCancel { get; set; }
		public string CompanyId { get; set; }
		public bool PremiaInUSD { get; set; }
		public bool IsEmptyMembers { get; set; }

		//public string PremiaSum { get; set; }
		//public string PremiaSumAll { get; set; }
		//public string People { get; set; }
		//public string PeopleAll { get; set; }
		//public string Zaralies { get; set; }

		public string Code { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }

		public string Status { get; set; }
        public int clrfg { get; set; }
    }
}
