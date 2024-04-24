using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models {
    public class Category {
        [Key]
        public int categoryID { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Title is required!")]
        public string title { get; set; }
        [Column(TypeName = "nvarchar(5)")]
        public string icon { get; set; } = "";
        [Column(TypeName = "nvarchar(10)")]
        public string type { get; set; } = "Expense";


        [NotMapped]
        public string? titleWithIcon { 
            get {
                return this.icon + " " + this.title;
            }
        }
    }
}
