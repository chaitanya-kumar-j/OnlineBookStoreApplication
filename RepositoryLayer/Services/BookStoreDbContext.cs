using CommonLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class BookStoreDbContext : DbContext
    {
        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<WishList> WishLists { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<AddressType> AddressTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.RegistrationTime)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Cart>()
                .HasKey(c => new { c.UserId, c.BookId });

            modelBuilder.Entity<WishList>()
                .HasKey(c => new { c.UserId, c.BookId });
        }
    }
}
