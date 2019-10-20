using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int? OwnerId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string ShortDescription { get; set; }

        [Required]
        [MaxLength(4000)]
        public string LongDescription { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture1 { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture2 { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture3 { get; set; }

        public bool isAvailable { get; set; }

        public bool isSold { get; set; }
    }
}
