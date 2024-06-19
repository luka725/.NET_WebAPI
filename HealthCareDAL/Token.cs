namespace HealthCareDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Token
    {
        public int TokenID { get; set; }

        public int? UserID { get; set; }

        [StringLength(255)]
        public string TokenValue { get; set; }

        public DateTime? ExpiryDateTime { get; set; }

        [StringLength(50)]
        public string TokenType { get; set; }

        public virtual User User { get; set; }
    }
}
