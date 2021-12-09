using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class WishListResponse
    {
        public int UserId { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public int Stock { get; set; }

        public double Price { get; set; }
    }
}
