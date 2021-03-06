using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    [Table("product_size")]
    public partial class ProductSize
    {
        public ProductSize()
        {
            Bag = new HashSet<Bag>();
            ProductOrder = new HashSet<ProductOrder>();
        }

        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }
        [Key]
        [Column("size", TypeName = "enum('36','37','38','39','40','41','42','43','44','45','46','47','48','50','52','54','56','XS','S','M','L','XL','2XL','3XL','4XL','5XL')")]
        public string Size { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductSize")]
        public virtual Product Product { get; set; }
        [InverseProperty("ProductSize")]
        public virtual ICollection<Bag> Bag { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<ProductOrder> ProductOrder { get; set; }
    }
}
