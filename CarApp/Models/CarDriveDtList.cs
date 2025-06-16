using System;
using System.ComponentModel.DataAnnotations;

namespace CarApp.Models
{
    public class CarDriveDtList
    {
        [Required(ErrorMessage = "DRDT_ID is required")]
        [StringLength(15, ErrorMessage = "DRDT_ID must be between {2} and {1} characters", MinimumLength = 1)]
        public string DRDT_ID { get; set; }

        [StringLength(15)]
        public string? INVOICE_NO { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? INVOICE_DT { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Invoice amount must be a non-negative number")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? INVOICE_AMT { get; set; }

        [StringLength(50)]
        [RegularExpression("^(STANDARD|EXPRESS|SPECIAL)$", ErrorMessage = "CAR_DR_TYPE must be 'STANDARD', 'EXPRESS', or 'SPECIAL'")]
        public string? CAR_DR_TYPE { get; set; }

        [StringLength(int.MaxValue)]
        public string? NOTES { get; set; }
    }
}