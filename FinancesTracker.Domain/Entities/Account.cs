using System.ComponentModel.DataAnnotations;

namespace FinancesTracker.Domain.Entities
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        private decimal _balance;
        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative.")]
        public decimal Balance
        {
            get => _balance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Balance), "Balance cannot be negative.");
                _balance = value;
            }
        }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Currency { get; set; } = string.Empty;

        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
