using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        [ForeignKey("AddressTypes")]
        public int AddressTypeId { get; set; }

        public string DoorNumber { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public int ZipCode { get; set; }
    }
}
