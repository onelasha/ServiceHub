using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class GIGridColumn
    {
        public string Title { get; set; }

        public string DataIndex { get; set; }
        public string DisplayField { get; set; }
        public string ValueField { get; set; }
        public string Width { get; set; }
        public string Flex { get; set; }
        public string ValueType { get; set; }

        public string Renderer { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFilter { get; set; }
        public bool IsNotColumn { get; set; }
        public bool IsHidden { get; set; }
        public bool IsMenuDisabled { get; set; }
        public bool IsGridSummaryRow { get; set; }
        public string SummaryRenderer { get; set; }
    }
}
