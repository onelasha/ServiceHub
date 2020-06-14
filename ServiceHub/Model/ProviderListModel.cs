using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class ProviderListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string ProviderType { get; set; }
        public string Comment { get; set; }
        public string TaxNo { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Region { get; set; }
        public string Raion { get; set; }
        public string Address { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public int clrfg { get; set; }
    }
}
