using System;
using System.ComponentModel.DataAnnotations;

namespace CarApp.Models
{
    public class CarDriveHeader
    {
        [Required(ErrorMessage = "CAR_DR_ID is required")]
        [StringLength(15, ErrorMessage = "CAR_DR_ID must be between {2} and {1} characters", MinimumLength = 1)]
        public string CAR_DR_ID { get; set; }

        [StringLength(15)]
        public string? CAR_ID { get; set; }

        [StringLength(15)]
        public string? ASSET_ID { get; set; }

        [StringLength(200)]
        public string? ASSET_NAME { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "OLD_INDEX_NUMBER must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? OLD_INDEX_NUMBER { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "NEW_INDEX_NUMBER must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? NEW_INDEX_NUMBER { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "INDEX_NUMBER must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? INDEX_NUMBER { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "POWER_RATE must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? POWER_RATE { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "POWER_RATE_INDEX must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? POWER_RATE_INDEX { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "CURR_POWER_RATE must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? CURR_POWER_RATE { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? INPUT_DT { get; set; }

        [StringLength(1)]
        [RegularExpression("[YN]", ErrorMessage = "ISLEAF must be either 'Y' or 'N'")]
        public string? ISLEAF { get; set; }

        [StringLength(15)]
        public string? PARENT_ID { get; set; }

        [StringLength(1000)]
        public string? NOTES { get; set; }

        [StringLength(1)]
        [RegularExpression("[AID]", ErrorMessage = "RECORD_STATUS must be 'A' (Active), 'I' (Inactive), or 'D' (Deleted)")]
        public string? RECORD_STATUS { get; set; }

        [StringLength(15)]
        public string? MAKER_ID { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? CREATE_DT { get; set; }

        [StringLength(50)]
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "AUTH_STATUS must be 'Pending', 'Approved', or 'Rejected'")]
        public string? AUTH_STATUS { get; set; }

        [StringLength(15)]
        public string? CHECKER_ID { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? APPROVE_DT { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "CAR_AMT must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? CAR_AMT { get; set; }

        [StringLength(15)]
        public string? INVOICE_NO { get; set; }

        [StringLength(int.MaxValue)]
        public string? DR_ROUTE { get; set; }
    }
}