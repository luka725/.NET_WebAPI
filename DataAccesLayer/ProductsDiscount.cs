namespace DataAccesLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductsDiscount
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public int DiscountID { get; set; }

        public virtual Discount Discount { get; set; }

        public virtual Product Product { get; set; }
    }
}
