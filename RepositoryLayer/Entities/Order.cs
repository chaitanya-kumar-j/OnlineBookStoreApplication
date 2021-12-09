using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public Address DeliveryAddress { get; set; }

        public Cart cart { get; set; }
    }
}
