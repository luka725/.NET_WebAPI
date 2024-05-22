namespace DataAccesLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int OrderDetailsID { get; set; }

        [Column(TypeName = "date")]
        public DateTime OrderDate { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }

        public virtual User User { get; set; }
    }
}
