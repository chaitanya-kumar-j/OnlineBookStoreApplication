using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entities
{
    [Keyless]
    public class Cart
    {
        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Books")]
        public int BookId { get; set; }

        [Required]
        public int NumberOfBooks { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double Amount { get; set; }
    }
}
