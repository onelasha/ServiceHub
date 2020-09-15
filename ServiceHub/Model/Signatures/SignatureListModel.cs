using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class SignatureListModel
    {
        public int RowNum { get; set; }
        public int SignatureId { get; set; }
        public string Application { get; set; }
        public string Position { get; set; }
        public string Project { get; set; }
        public int AuthorLevel { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
        public int clrfg { get; set; }
    }
}
