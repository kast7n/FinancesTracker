using System;
using System.ComponentModel.DataAnnotations;

namespace FinancesTracker.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [RegularExpression("Income|Expense", ErrorMessage = "Type must be 'Income' or 'Expense'.")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Category { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public int AccountID { get; set; }
        public Account? Account { get; set; }

        public string? Notes { get; set; }
    }
}
