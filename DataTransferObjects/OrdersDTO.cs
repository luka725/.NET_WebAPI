using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataTransferObjects
{
    public class OrdersDTO
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int OrderDetailsID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}