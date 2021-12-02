namespace BusinessLogicLayer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using BusinessLogicLayer.Interfaces;
    using CommonLayer.Models;
    using RepositoryLayer.Interfaces;

    public class BookStoreLogic : IBookStoreLogic
    {
        private IBookStoreData _bookStoreData;

        public BookStoreLogic(IBookStoreData bookStoreData)
        {
            this._bookStoreData = bookStoreData;
        }

        public async Task<Book> AddBook(Book newBook)
        {
            try
            {
                return await _bookStoreData.AddBook(newBook);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Book>> DeleteBook(int bookId)
        {
            try
            {
                return await _bookStoreData.DeleteBook(bookId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                return await _bookStoreData.GetAllBooks();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Book> GetBook(int bookId)
        {
            try
            {
                return await _bookStoreData.GetBook(bookId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Book> UpdateBook(int bookId, Book updatedBook)
        {
            try
            {
                return await _bookStoreData.UpdateBook(bookId, updatedBook);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<User> UserLogin(UserLogin userLogin)
        {
            try
            {
                return await _bookStoreData.UserLogin(userLogin);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<User> UserRegistration(User user)
        {
            try
            {
                return await _bookStoreData.UserRegistration(user);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
