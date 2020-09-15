using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class ClientOrgListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string CodePin { get; set; }
        public string Passport { get; set; }
        public string Description { get; set; }
        public string Entity { get; set; }
        public string Dob { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public bool IsVip { get; set; }
        public int clrfg { get; set; }
    }
}
