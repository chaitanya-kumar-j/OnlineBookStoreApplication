namespace BusinessLogicLayer.Interfaces
{
    using CommonLayer.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBookStoreLogic
    {
        Task<User> UserRegistration(User user);
        Task<User> UserLogin(UserLogin userLogin);
        Task<List<Book>> GetAllBooks();
        Task<Book> GetBook(int bookId);
        Task<Book> UpdateBook(int bookId, Book updatedBook);
        Task<Book> AddBook(Book newBook);
        Task<List<Book>> DeleteBook(int bookId);
        Task<List<Cart>> GetAllBooksInCart(int userId);
        Task<List<Cart>> DeleteABookInCart(int userId, int bookId);
        Task<Cart> UpdateABookInCart(int userId, int bookId, int numberOfBooks);
        Task<Cart> AddABookToCart(int userId, int bookId, int numberOfBooks);
        Task<Cart> GetABookInCart(int userId, int bookId);
    }
}
