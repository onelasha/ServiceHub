using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class StaffListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Pin { get; set; }
        public string Passport { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string LastTimeOn { get; set; }
        public int clrfg { get; set; }
    }
}
