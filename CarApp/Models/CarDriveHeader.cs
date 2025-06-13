using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class CarDriveHeader
    {
        public string CAR_DR_ID { get; set; }
        public string? CAR_ID { get; set; }
        public string? ASSET_ID { get; set; }
        public string? ASSET_NAME { get; set; }
        public decimal? OLD_INDEX_NUMBER { get; set; }
        public decimal? NEW_INDEX_NUMBER { get; set; }
        public decimal? INDEX_NUMBER { get; set; }
        public decimal? POWER_RATE { get; set; }
        public decimal? POWER_RATE_INDEX { get; set; }
        public decimal? CURR_POWER_RATE { get; set; }
        public DateTime? INPUT_DT { get; set; }
        public string? ISLEAF { get; set; } 
        public string? PARENT_ID { get; set; } 
        public string? NOTES { get; set; } 
        public string? RECORD_STATUS { get; set; } 
        public string? MAKER_ID { get; set; } 
        public DateTime? CREATE_DT { get; set; }
        public string? AUTH_STATUS { get; set; } 
        public string? CHECKER_ID { get; set; } 
        public DateTime? APPROVE_DT { get; set; }
        public decimal? CAR_AMT { get; set; } 
        public string? INVOICE_NO { get; set; } 
        public string? DR_ROUTE { get; set; } 
    }

}