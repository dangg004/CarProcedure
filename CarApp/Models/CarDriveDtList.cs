using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class CarDriveDtList
    {
        public string DRDT_ID { get; set; }
        public string? INVOICE_NO { get; set; }
        public DateTime? INVOICE_DT { get; set; }
        public decimal? INVOICE_AMT { get; set; }
        public string? CAR_DR_TYPE { get; set; }
        public string? NOTES { get; set; }
    }
}