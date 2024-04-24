using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models {
    public class Transaction {
        [Key]
        public int transactionID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category!")]
        public int categoryID { get; set; }
        public Category? category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0!")]
        public int amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? note { get; set; }

        public DateTime date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? categoryTitleWithIcon {
            get {
                return category == null ? "" : category.icon + " " + category.title;
            }
        }
        [NotMapped]
        public string? formatedAmount {
            get {
                return ((category == null || category.type  == "Expense") ? "- " : "+ ") + amount.ToString("C0");
            }
        }
    }
}
