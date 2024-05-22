﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTransferObjects
{
    public class OrderDetailsDTO
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}