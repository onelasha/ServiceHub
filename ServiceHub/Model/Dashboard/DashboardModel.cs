using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class PieColorModel
    {
        public string clrfg { get; set; }
    }
    public class SaxeobaModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string clrfg { get; set; }
    }
    public class PieDataModel
    {
        public int StatisticsId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string clrfg { get; set; }
    }
    public class ActivityModel
    {
        public int ActivityId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public string DataTimeOn { get; set; }
        public string clrfg { get; set; }
    }
    public class EntityPropertyModel
    {
        public int EntityPropertyId { get; set; }
        public int EntityId { get; set; }
        public string PropertyType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string clrfg { get; set; }
    }
    public class EntityModel
    {
        public int EntityId { get; set; }

        public string Description { get; set; }
        public string PIN { get; set; }
        public string Number { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string DateCancel { get; set; }
        public string Operator { get; set; }
        public string DataTimeOn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }

        public int RecState { get; set; }
        public string Status { get; set; }
        public string clrfg { get; set; }
        public string clrbg { get; set; }
        //public List<EntityPropertyModel> EntityPropertyList { get; set; }
    }

    public class DashboardModel
    {
        public List<SaxeobaModel> SaxeobaList { get; set; }
        public List<PieDataModel> PieDataList { get; set; }
        public List<ActivityModel> ActivityList { get; set; }
        public List<EntityModel> EntityList { get; set; }
        public List<PieColorModel> PieColorList { get; set; }
    }
}
