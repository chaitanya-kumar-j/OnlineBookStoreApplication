using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class CartResponse
    {
        public int UserId { get; set; }

        public string BookTitle { get; set; }

        public int Quantity { get; set; }

        public double Amount { get; set; }
    }
}
