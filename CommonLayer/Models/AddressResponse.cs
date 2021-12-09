using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    public class AddressResponse
    {
        public int UserId { get; set; }

        public int AddressId { get; set; }

        public string TypeOfAddress { get; set; }

        public string DoorNumber { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public int ZipCode { get; set; }
    }
}
