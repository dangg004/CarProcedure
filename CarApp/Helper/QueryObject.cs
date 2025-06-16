using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarApp.Helper
{
    public class QueryObject
    {
        public string? carId { get; set; } = null;
        public string? assetId { get; set; } = null;
        public string? invoiceNo { get; set; } = null;
        public string? driveNote { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}