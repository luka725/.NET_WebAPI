using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTransferObjects
{
    public class DiscountsDTO
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public double DiscountPrecentage { get; set; }
        public DateTime ValidityPeriod { get; set; }
    }
}