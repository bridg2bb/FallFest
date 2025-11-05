using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FallFest.Models
{
    public class Item
    {
        [Key]
        [Column("ItemID")]
        public int ItemID { get; set; }

        [Column("ItemName")]
        public string ItemName { get; set; }

        [Column("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int OrderTypeId { get; set; }

        public bool Enabled { get; set; }
    }

   
    public class ItemTotalViewModel
    {
        public string ItemName;
        public decimal QuantitySold = 0;
        public decimal TotalRevenue = 0;
    }
}

