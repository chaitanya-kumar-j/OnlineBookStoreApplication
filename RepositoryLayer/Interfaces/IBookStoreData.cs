namespace RepositoryLayer.Interfaces
{
    using CommonLayer.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBookStoreData
    {
        Task<User> UserRegistration(User user);
        Task<User> UserLogin(UserLogin userLogin);
        Task<Book> AddBook(Book newBook);
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBook(int bookId);
        Task<Book> UpdateBook(int bookId, Book updatedBook);
        Task<List<Book>> DeleteBook(int bookId);
        Task<Cart> AddABookToCart(int userId, int bookId, int numberOfBooks);
        Task<Cart> UpdateABookInCart(int userId, int bookId, int numberOfBooks);
        Task<List<Cart>> GetAllBooksInCart(int userId);
        Task<Cart> GetABookInCart(int userId, int bookId);
        Task<List<Cart>> DeleteABookInCart(int userId, int bookId);
    }
}
