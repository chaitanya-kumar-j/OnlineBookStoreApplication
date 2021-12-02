namespace RepositoryLayer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommonLayer.Models;
    using Microsoft.EntityFrameworkCore;
    using RepositoryLayer.Interfaces;

    public class BookStoreData : IBookStoreData
    {
        public readonly BookStoreDbContext _bookStoreDbContext;
        public readonly IPwdEncryptDecryptService _pwdEncryptDecrypt;
        public BookStoreData(BookStoreDbContext bookStoreDbContext, IPwdEncryptDecryptService pwdEncryptDecrypt)
        {
            this._bookStoreDbContext = bookStoreDbContext;
            this._pwdEncryptDecrypt = pwdEncryptDecrypt;
        }

        public async Task<Book> AddBook(Book newBook)
        {
            await this._bookStoreDbContext.Books.AddAsync(newBook);
            await this._bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Books.Where(u => u.Title == newBook.Title).FirstOrDefaultAsync();
        }

        public async Task<List<Book>> DeleteBook(int bookId)
        {
            var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            this._bookStoreDbContext.Books.Remove(book);
            await this._bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Books.ToListAsync();
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookStoreDbContext.Books.ToListAsync();
        }

        public async Task<Book> GetBook(int bookId)
        {
            return await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
        }

        public async Task<Book> UpdateBook(int bookId, Book updatedBook)
        {
            var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Category = updatedBook.Category;
            book.Stock = updatedBook.Stock;
            book.Price = updatedBook.Price;
            await _bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
        }

        public async Task<User> UserLogin(UserLogin userLogin)
        {
            var user = await _bookStoreDbContext.Users.Where(u => u.Email == userLogin.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                var password = _pwdEncryptDecrypt.DecryptPassword(user.Password);
                if(password == userLogin.Password)
                {
                    return user;
                }
                throw new Exception("Password entered is wrong");
            }
            throw new Exception("No user registered with given Email id");
        }

        public async Task<User> UserRegistration(User user)
        {
            var password = _pwdEncryptDecrypt.EncryptPassword(user.Password);
            user.Password = password;
            await this._bookStoreDbContext.Users.AddAsync(user);
            await this._bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
        }
    }
}
