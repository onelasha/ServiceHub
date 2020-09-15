using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class SignatureModel
    {
            public int SignatureId { get; set; }
            public int DocTypeId { get; set; }
            public int PositionId { get; set; }
            public int ProjectId { get; set; }
            public string DocType { get; set; }
            public string Position { get; set; }
            public string Project { get; set; }
            public int Level { get; set; }
            public decimal MinAmount { get; set; }
            public decimal MaxAmount { get; set; }
    }


}
