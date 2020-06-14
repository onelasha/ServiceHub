using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class ExchangeRateListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
        public string Iso { get; set; }
        public int Unit { get; set; }
        public decimal Rate { get; set; }
        public int clrfg { get; set; }
    }
}
