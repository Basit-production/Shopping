using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ahmed_mart.Dtos.v1.OrderStatusDto
{
    public class UpdateOrderStatusDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
    }
}
