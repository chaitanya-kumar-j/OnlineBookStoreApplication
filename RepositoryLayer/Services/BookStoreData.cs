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
    using RepositoryLayer.Entities;

    public class BookStoreData : IBookStoreData
    {
        public readonly BookStoreDbContext _bookStoreDbContext;
        public readonly IPwdEncryptDecryptService _pwdEncryptDecrypt;
        public BookStoreData(BookStoreDbContext bookStoreDbContext, IPwdEncryptDecryptService pwdEncryptDecrypt)
        {
            this._bookStoreDbContext = bookStoreDbContext;
            this._pwdEncryptDecrypt = pwdEncryptDecrypt;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookStoreDbContext.Books.ToListAsync();
        }

        public async Task<Book> GetBook(int bookId)
        {
            var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            if (book !=null)
            {
                return book;
            }
            throw new Exception("No book available with given book id to view.");
        }

        public async Task<Book> AddBook(Book newBook)
        {
            await this._bookStoreDbContext.Books.AddAsync(newBook);
            await this._bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Books.Where(u => u.Title == newBook.Title).FirstOrDefaultAsync();
        }

        public async Task<Book> UpdateBook(int bookId, Book updatedBook)
        {
            var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            if (book != null)
            {
                book.Title = updatedBook.Title;
                book.Author = updatedBook.Author;
                book.Category = updatedBook.Category;
                book.Stock = updatedBook.Stock;
                book.Price = updatedBook.Price;
                await _bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            }
            throw new Exception("No book available with given book id to update.");
        }

        public async Task<List<Book>> DeleteBook(int bookId)
        {
            var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
            if (book != null)
            {
                this._bookStoreDbContext.Books.Remove(book);
                await this._bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.Books.ToListAsync();
            }
            throw new Exception("No book available with given book id to delete.");
            
        }

        public async Task<User> UserRegistration(User user)
        {
            var password = _pwdEncryptDecrypt.EncryptPassword(user.Password);
            user.Password = password;
            await this._bookStoreDbContext.Users.AddAsync(user);
            await this._bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
        }

        public async Task<User> UserLogin(UserLogin userLogin)
        {
            var user = await _bookStoreDbContext.Users.Where(u => u.Email == userLogin.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                var password = _pwdEncryptDecrypt.DecryptPassword(user.Password);
                if (password == userLogin.Password)
                {
                    return user;
                }
                throw new Exception("Password entered is wrong");
            }
            throw new Exception("No user registered with given Email id");
        }

        public async Task<List<CartResponse>> GetAllBooksInCart(int userId)
        {
            return await _bookStoreDbContext.Carts
                .Where(c=>c.UserId == userId)
                .Join(_bookStoreDbContext.Books,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new CartResponse 
                { 
                    Amount = c.Amount, 
                    UserId = c.UserId, 
                    BookTitle = b.Title, 
                    Quantity = c.NumberOfBooks 
                }).ToListAsync();
        }

        public async Task<CartResponse> GetABookInCart(int userId, int bookId)
        {
            var book = await _bookStoreDbContext.Carts.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (book != null)
            {
                return await _bookStoreDbContext.Carts
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books
                .Where(b => b.BookId == bookId),
                c => c.BookId,
                b => b.BookId,
                (c, b) => new CartResponse
                {
                    Amount = c.Amount,
                    UserId = c.UserId,
                    BookTitle = b.Title,
                    Quantity = c.NumberOfBooks
                }).FirstOrDefaultAsync();
            }
            throw new Exception("No book in cart with selected book id.");
        }

        public async Task<CartResponse> AddABookToCart(int userId, int bookId, int numberOfBooks)
        {
            var cart = await _bookStoreDbContext.Carts.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (cart.Equals(null))
            {
                var book = await _bookStoreDbContext.Books.Where(b => b.BookId == bookId).FirstOrDefaultAsync();
                if (book.Equals(null))
                {
                    throw new Exception("No book available in book store with selected book id");
                }
                var price = book.Price;
                if (book.Stock >= numberOfBooks)
                {
                    cart = new Cart()
                    {
                        UserId = userId,
                        BookId = bookId,
                        NumberOfBooks = numberOfBooks,
                        Amount = numberOfBooks * price
                    };
                    await _bookStoreDbContext.Carts.AddAsync(cart);
                    await _bookStoreDbContext.SaveChangesAsync();
                    return await _bookStoreDbContext.Carts
                       .Where(c => c.UserId == userId)
                       .Join(_bookStoreDbContext.Books
                       .Where(b => b.BookId == bookId),
                       c => c.BookId,
                       b => b.BookId,
                       (c, b) => new CartResponse
                       {
                           Amount = c.Amount,
                           UserId = c.UserId,
                           BookTitle = b.Title,
                           Quantity = c.NumberOfBooks
                       }).FirstOrDefaultAsync();
                }
                else
                {
                    if (book.Stock > 0)
                    {
                        throw new Exception("Selected Quantity is more than stock available.");
                    }
                    throw new Exception("Sorry!...Out of Stock.");
                }
            }
            return await UpdateABookInCart(userId, bookId, numberOfBooks + cart.NumberOfBooks);
        }

        public async Task<CartResponse> UpdateABookInCart(int userId, int bookId, int numberOfBooks)
        {
            var cart = await _bookStoreDbContext.Carts.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (cart.Equals(null))
            {
                throw new Exception("No book in cart with selected book id.");
            }
            cart.Amount = (cart.Amount / cart.NumberOfBooks) * (numberOfBooks);
            cart.NumberOfBooks = numberOfBooks;
            _bookStoreDbContext.Carts.Update(cart);
            await _bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Carts
                    .Where(c => c.UserId == userId)
                    .Join(_bookStoreDbContext.Books
                    .Where(b => b.BookId == bookId),
                    c => c.BookId,
                    b => b.BookId,
                    (c, b) => new CartResponse
                    {
                        Amount = c.Amount,
                        UserId = c.UserId,
                        BookTitle = b.Title,
                        Quantity = c.NumberOfBooks
                    }).FirstOrDefaultAsync();
        }

        public async Task<List<CartResponse>> DeleteABookInCart(int userId, int bookId)
        {
            var cart = await _bookStoreDbContext.Carts.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (cart.Equals(null))
            {
                throw new Exception("No book in cart with selected book id.");
            }
            _bookStoreDbContext.Carts.Remove(cart);
            await _bookStoreDbContext.SaveChangesAsync();
            return await _bookStoreDbContext.Carts
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new CartResponse
                {
                    Amount = c.Amount,
                    UserId = c.UserId,
                    BookTitle = b.Title,
                    Quantity = c.NumberOfBooks
                }).ToListAsync();
        }

        public async Task<List<WishListResponse>> GetAllBooksInWishList(int userId)
        {
            return await _bookStoreDbContext.WishLists
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new WishListResponse
                {
                    UserId = c.UserId,
                    BookTitle = b.Title,
                }).ToListAsync();
        }

        public async Task<WishListResponse> GetABookInWishList(int userId, int bookId)
        {
            var book = await _bookStoreDbContext.WishLists.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (book != null)
            {
                return await _bookStoreDbContext.WishLists
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books
                .Where(b => b.BookId == bookId),
                c => c.BookId,
                b => b.BookId,
                (c, b) => new WishListResponse
                {
                    UserId = c.UserId,
                    BookTitle = b.Title,
                }).FirstOrDefaultAsync();
            }
            throw new Exception("No book in wishlist with selected book id.");
        }

        public async Task<WishListResponse> AddABookToWishList(int userId, int bookId)
        {
            var wishListBook = await _bookStoreDbContext.WishLists.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (wishListBook == null)
            {
                var book = await _bookStoreDbContext.Books.Where(b => b.BookId == bookId).FirstOrDefaultAsync();
                if (book.Equals(null))
                {
                    throw new Exception("No book available in book store with selected book id");
                }
                wishListBook = new WishList()
                {
                    UserId = userId,
                    BookId = bookId,
                };
                await _bookStoreDbContext.WishLists.AddAsync(wishListBook);
                await _bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.WishLists
                   .Where(c => c.UserId == userId)
                   .Join(_bookStoreDbContext.Books
                   .Where(b => b.BookId == bookId),
                   c => c.BookId,
                   b => b.BookId,
                   (c, b) => new WishListResponse
                   {
                       UserId = c.UserId,
                       BookTitle = b.Title,
                   }).FirstOrDefaultAsync();
            }
            return await _bookStoreDbContext.WishLists
                   .Where(c => c.UserId == userId)
                   .Join(_bookStoreDbContext.Books
                   .Where(b => b.BookId == bookId),
                   c => c.BookId,
                   b => b.BookId,
                   (c, b) => new WishListResponse
                   {
                       UserId = c.UserId,
                       BookTitle = b.Title,
                   }).FirstOrDefaultAsync();
        }

        public async Task<List<WishListResponse>> RemoveABookFromWishList(int userId, int bookId)
        {
            var wishListBook = await _bookStoreDbContext.WishLists.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (wishListBook != null)
            {
                _bookStoreDbContext.WishLists.Remove(wishListBook);
                await _bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.WishLists
                    .Where(c => c.UserId == userId)
                    .Join(_bookStoreDbContext.Books,
                    c => c.BookId,
                    b => b.BookId,
                    (c, b) => new WishListResponse
                    {
                        UserId = c.UserId,
                        BookTitle = b.Title,
                    }).ToListAsync();
            }
            throw new Exception("Book not yet added to wishlist.");
        }

        public async Task<List<WishListResponse>> MoveToCart(int userId, int bookId)
        {
            var wishListBook = await _bookStoreDbContext.WishLists.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (wishListBook != null)
            {
                _bookStoreDbContext.WishLists.Remove(wishListBook);
                await _bookStoreDbContext.SaveChangesAsync();
                await AddABookToCart(userId, bookId, 1);
                return await _bookStoreDbContext.WishLists
                    .Where(c => c.UserId == userId)
                    .Join(_bookStoreDbContext.Books,
                    c => c.BookId,
                    b => b.BookId,
                    (c, b) => new WishListResponse
                    {
                        UserId = c.UserId,
                        BookTitle = b.Title,
                    }).ToListAsync();
            }
            throw new Exception("Book not yet added to wishlist.");
        }

        public async Task<List<CartResponse>> MoveABookToWishList(int userId, int bookId)
        {
            var cart = await _bookStoreDbContext.Carts.Where(c => c.UserId == userId && c.BookId == bookId).FirstOrDefaultAsync();
            if (cart.Equals(null))
            {
                throw new Exception("No book in cart with selected book id.");
            }
            _bookStoreDbContext.Carts.Remove(cart);
            await _bookStoreDbContext.SaveChangesAsync();
            await AddABookToWishList(userId, bookId);
            return await _bookStoreDbContext.Carts
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new CartResponse
                {
                    Amount = c.Amount,
                    UserId = c.UserId,
                    BookTitle = b.Title,
                    Quantity = c.NumberOfBooks
                }).ToListAsync();
        }
    }
}
