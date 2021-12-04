namespace BusinessLogicLayer.Interfaces
{
    using CommonLayer.Models;
    using RepositoryLayer.Entities;
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
        Task<Book> AddBook(Book newBook);
        Task<Book> UpdateBook(int bookId, Book updatedBook);
        Task<List<Book>> DeleteBook(int bookId);
        Task<List<CartResponse>> GetAllBooksInCart(int userId);
        Task<CartResponse> GetABookInCart(int userId, int bookId);
        Task<CartResponse> AddABookToCart(int userId, int bookId, int numberOfBooks);
        Task<CartResponse> UpdateABookInCart(int userId, int bookId, int numberOfBooks);
        Task<List<CartResponse>> DeleteABookInCart(int userId, int bookId);
        Task<List<WishListResponse>> GetAllBooksInWishList(int userId);
        Task<WishListResponse> GetABookInWishList(int userId, int bookId);
        Task<WishListResponse> AddABookToWishList(int userId, int bookId);
        Task<List<WishListResponse>> RemoveABookFromWishList(int userId, int bookId);
        Task<List<WishListResponse>> MoveToCart(int userId, int bookId);
        Task<List<CartResponse>> MoveABookToWishList(int userId, int bookId);
    }
}
