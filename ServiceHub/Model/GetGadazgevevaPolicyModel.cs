using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//reestrContractList
namespace ServiceHub.Model
{
    public class GetGadazgevevaPolicyModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }


		public string Date { get; set; }
		public string Policy { get; set; }
		public string ClientName { get; set; }
		public string ReisnurerName { get; set; }
		public string Premium { get; set; }
		public string PremiumISO { get; set; }
		public string Commission { get; set; }
		public string CommissionISO { get; set; }
		public string StatusName { get; set; }
		public string Amount { get; set; }
		public string ISO { get; set; }
		public string TPLLimit { get; set; }
		public string TPLISO { get; set; }
		public string MALimit { get; set; }
		public string MAISO { get; set; }


		public string Status { get; set; }
        public int clrfg { get; set; }
    }
}
