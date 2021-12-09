using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RepositoryLayer.Entities
{
    public class AddressType
    {
        [Key]
        public int AddressTypeId { get; set; }

        public string TypeOfAddress { get; set; }
    }
}
