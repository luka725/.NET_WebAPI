using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTransferObjects
{
    public class ProductsDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryID { get; set; }

    }
}