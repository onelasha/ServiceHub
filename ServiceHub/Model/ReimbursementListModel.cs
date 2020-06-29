using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Model
{
    public class ReimbursementListModel
    {
        public int RowNum { get; set; }
        public int Id { get; set; }
        public string AccountinNo { get; set; }
        public string ClaimNo { get; set; }
        public string OrderNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceDateOffer { get; set; }
        public string InvoiceDateImplementation { get; set; }
        public string InvoiceDateIssue { get; set; }
        public string InvoiceAmountPre { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceDoscountAmount { get; set; }
        public string InvoiceLossAmount { get; set; }
        public string InvoiceTotalCorrectedAmount { get; set; }
        public string InvoiceCorrectedAmount { get; set; }
        public string Status { get; set; }
        public int clrfg { get; set; }
    }
}
