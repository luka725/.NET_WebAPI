namespace DataAccesLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Discount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Discount()
        {
            ProductsDiscounts = new HashSet<ProductsDiscount>();
        }

        public int ID { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Description { get; set; }

        public double DiscountPrecentage { get; set; }

        [Column(TypeName = "date")]
        public DateTime ValidityPeriod { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductsDiscount> ProductsDiscounts { get; set; }
    }
}
