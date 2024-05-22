using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTransferObjects
{
    public class ProductsDiscountsDTO
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int DiscountID { get; set; }
    }
}