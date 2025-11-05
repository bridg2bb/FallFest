// File: FallFest.Models/ItemType.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FallFest.Models
{
    public class ItemType
    {
        [Key]
        public int ItemTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemTypeName { get; set; }

        [Required]
        public int SortOrder { get; set; }

        // Collection navigation property for related Items
        public ICollection<Item> Items { get; set; }
    }
}