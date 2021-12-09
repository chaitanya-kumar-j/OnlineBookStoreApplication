namespace RepositoryLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [ForeignKey("Addresses")]
        public int AddressId { get; set; }

        [Required]
        public int ZipCode { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime RegistrationTime { get; set; }
    }
}
