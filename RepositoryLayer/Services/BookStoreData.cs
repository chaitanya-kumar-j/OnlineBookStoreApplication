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
            try
            {
                return await _bookStoreDbContext.Books.ToListAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<Book> GetBook(int bookId)
        {
            try
            {
                var book = await _bookStoreDbContext.Books.Where(u => u.BookId == bookId).FirstOrDefaultAsync();
                if (book != null)
                {
                    return book;
                }
                throw new Exception("No book available with given book id to view.");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<Book> AddBook(Book newBook)
        {
            try
            {
                await this._bookStoreDbContext.Books.AddAsync(newBook);
                await this._bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.Books.Where(u => u.Title == newBook.Title).FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<Book> UpdateBook(int bookId, Book updatedBook)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Book>> DeleteBook(int bookId)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<User> UserRegistration(User user)
        {
            try
            {
                var password = _pwdEncryptDecrypt.EncryptPassword(user.Password);
                user.Password = password;
                await this._bookStoreDbContext.Users.AddAsync(user);
                await this._bookStoreDbContext.SaveChangesAsync();
                return await _bookStoreDbContext.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<User> UserLogin(UserLogin userLogin)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<CartResponse>> GetAllBooksInCart(int userId)
        {
            try
            {
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<CartResponse> GetABookInCart(int userId, int bookId)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<CartResponse> AddABookToCart(int userId, int bookId, int numberOfBooks)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<CartResponse> UpdateABookInCart(int userId, int bookId, int numberOfBooks)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<CartResponse>> DeleteABookInCart(int userId, int bookId)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<WishListResponse>> GetAllBooksInWishList(int userId)
        {
            try
            {
                return await _bookStoreDbContext.WishLists
                .Where(c => c.UserId == userId)
                .Join(_bookStoreDbContext.Books,
                c => c.BookId,
                b => b.BookId,
                (c, b) => new WishListResponse
                {
                    UserId = c.UserId,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    Price = b.Price,
                    Description = b.Description,
                    Stock = b.Stock

                }).ToListAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<WishListResponse> GetABookInWishList(int userId, int bookId)
        {
            try
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
                        Title = b.Title,
                        Author = b.Author,
                        Category = b.Category,
                        Price = b.Price,
                        Description = b.Description,
                        Stock = b.Stock
                    }).FirstOrDefaultAsync();
                }
                throw new Exception("No book in wishlist with selected book id.");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<WishListResponse> AddABookToWishList(int userId, int bookId)
        {
            try
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
                           Title = b.Title,
                           Author = b.Author,
                           Category = b.Category,
                           Price = b.Price,
                           Description = b.Description,
                           Stock = b.Stock
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
                           Title = b.Title,
                           Author = b.Author,
                           Category = b.Category,
                           Price = b.Price,
                           Description = b.Description,
                           Stock = b.Stock
                       }).FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<WishListResponse>> RemoveABookFromWishList(int userId, int bookId)
        {
            try
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
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            Price = b.Price,
                            Description = b.Description,
                            Stock = b.Stock
                        }).ToListAsync();
                }
                throw new Exception("Book not yet added to wishlist.");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<WishListResponse>> MoveToCart(int userId, int bookId)
        {
            try
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
                            Title = b.Title,
                            Author = b.Author,
                            Category = b.Category,
                            Price = b.Price,
                            Description = b.Description,
                            Stock = b.Stock
                        }).ToListAsync();
                }
                throw new Exception("Book not yet added to wishlist.");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<CartResponse>> MoveABookToWishList(int userId, int bookId)
        {
            try
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
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<AddressResponse>> GetAllAddresses(int userId)
        {
            try
            {
                return await _bookStoreDbContext.Addresses
                .Where(a => a.UserId == userId)
                .Join(_bookStoreDbContext.AddressTypes,
                a => a.AddressTypeId,
                b => b.AddressTypeId,
                (a, b) => new AddressResponse
                {
                    UserId = a.UserId,
                    AddressId = a.AddressId,
                    TypeOfAddress = b.TypeOfAddress,
                    DoorNumber = a.DoorNumber,
                    State = a.State,
                    Street = a.Street,
                    City = a.City,
                    ZipCode = a.ZipCode

                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AddressResponse> GetAnAddress(int userId, int addressId)
        {
            try
            {
                return await _bookStoreDbContext.Addresses
                .Where(a => a.UserId == userId && a.AddressId == addressId)
                .Join(_bookStoreDbContext.AddressTypes,
                a => a.AddressTypeId,
                b => b.AddressTypeId,
                (a, b) => new AddressResponse
                {
                    UserId = a.UserId,
                    AddressId = a.AddressId,
                    TypeOfAddress = b.TypeOfAddress,
                    DoorNumber = a.DoorNumber,
                    State = a.State,
                    Street = a.Street,
                    City = a.City,
                    ZipCode = a.ZipCode

                }).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<AddressResponse>> GetAllAddressesOfAType(int userId, int addressTypeId)
        {
            try
            {
                return await _bookStoreDbContext.Addresses
                .Where(a => a.UserId == userId)
                .Join(_bookStoreDbContext.AddressTypes
                .Where(b => b.AddressTypeId == addressTypeId),
                a => a.AddressTypeId,
                b => b.AddressTypeId,
                (a, b) => new AddressResponse
                {
                    UserId = a.UserId,
                    AddressId = a.AddressId,
                    TypeOfAddress = b.TypeOfAddress,
                    DoorNumber = a.DoorNumber,
                    State = a.State,
                    Street = a.Street,
                    City = a.City,
                    ZipCode = a.ZipCode
                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AddressResponse> AddAnAddress(int userId, AddressInput inputAddress)
        {
            try
            {
                int addressTypeId;
                Address address;
                var addressType = await _bookStoreDbContext.AddressTypes
                .Where(t => t.TypeOfAddress.ToUpper() == inputAddress.TypeOfAddress.ToUpper()).FirstOrDefaultAsync();
                if (addressType == null)
                {
                    await _bookStoreDbContext.AddressTypes
                        .AddAsync(new AddressType { TypeOfAddress = inputAddress.TypeOfAddress.ToUpper() });
                    await _bookStoreDbContext.SaveChangesAsync();
                    addressType = await _bookStoreDbContext.AddressTypes
                        .Where(t => t.TypeOfAddress.ToUpper() == inputAddress.TypeOfAddress.ToUpper()).FirstOrDefaultAsync();
                    addressTypeId = addressType.AddressTypeId;
                    await _bookStoreDbContext.Addresses
                        .AddAsync(new Address { UserId = userId,
                        AddressTypeId = addressTypeId,
                        DoorNumber = inputAddress.DoorNumber,
                        State = inputAddress.State,
                        Street = inputAddress.Street,
                        City = inputAddress.City,
                        ZipCode = inputAddress.ZipCode});
                    await _bookStoreDbContext.SaveChangesAsync();
                    address = await _bookStoreDbContext.Addresses
                        .Where(a => a.UserId == userId &&
                        a.AddressTypeId == addressTypeId &&
                        a.DoorNumber.ToUpper() == inputAddress.DoorNumber.ToUpper() &&
                        a.Street.ToUpper() == inputAddress.Street.ToUpper() &&
                        a.City.ToUpper() == inputAddress.City.ToUpper() &&
                        a.State.ToUpper() == inputAddress.State.ToUpper() &&
                        a.ZipCode == inputAddress.ZipCode).FirstOrDefaultAsync();
                    return await GetAnAddress(userId, address.AddressId);
                }
                addressTypeId = addressType.AddressTypeId;
                address = await _bookStoreDbContext.Addresses
                    .Where(a => a.UserId == userId &&
                    a.AddressTypeId == addressTypeId &&
                    a.DoorNumber.ToUpper() == inputAddress.DoorNumber.ToUpper() &&
                    a.Street.ToUpper() == inputAddress.Street.ToUpper() &&
                    a.City.ToUpper() == inputAddress.City.ToUpper() &&
                    a.State.ToUpper() == inputAddress.State.ToUpper() &&
                    a.ZipCode == inputAddress.ZipCode).FirstOrDefaultAsync();
                if (address != null)
                {
                    throw new Exception("Address already Exists.");
                }
                await _bookStoreDbContext.Addresses
                    .AddAsync(new Address
                    {
                        UserId = userId,
                        AddressTypeId = addressTypeId,
                        DoorNumber = inputAddress.DoorNumber,
                        State = inputAddress.State,
                        Street = inputAddress.Street,
                        City = inputAddress.City,
                        ZipCode = inputAddress.ZipCode
                    });
                await _bookStoreDbContext.SaveChangesAsync();
                address = await _bookStoreDbContext.Addresses
                        .Where(a => a.UserId == userId &&
                        a.AddressTypeId == addressTypeId &&
                        a.DoorNumber.ToUpper() == inputAddress.DoorNumber.ToUpper() &&
                        a.Street.ToUpper() == inputAddress.Street.ToUpper() &&
                        a.City.ToUpper() == inputAddress.City.ToUpper() &&
                        a.State.ToUpper() == inputAddress.State.ToUpper() &&
                        a.ZipCode == inputAddress.ZipCode).FirstOrDefaultAsync();
                return await GetAnAddress(userId, address.AddressId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AddressResponse> UpdateAnAddress(int userId, int addressId, AddressInput updatedAddress)
        {
            try
            {
                AddressType addressType;
                Address address;
                address = await _bookStoreDbContext.Addresses
                .Where(a => a.AddressId == addressId).FirstOrDefaultAsync();
                if (address == null)
                {
                    throw new Exception("Address not found");
                }
                addressType = await _bookStoreDbContext.AddressTypes
                    .Where(t => t.TypeOfAddress.ToUpper() == updatedAddress.TypeOfAddress.ToUpper()).FirstOrDefaultAsync();
                if (addressType == null)
                {
                    await _bookStoreDbContext.AddressTypes
                            .AddAsync(new AddressType { TypeOfAddress = updatedAddress.TypeOfAddress.ToUpper() });
                    await _bookStoreDbContext.SaveChangesAsync();
                    addressType = await _bookStoreDbContext.AddressTypes
                        .Where(t => t.TypeOfAddress.ToUpper() == updatedAddress.TypeOfAddress.ToUpper()).FirstOrDefaultAsync();
                }
                address.AddressTypeId = addressType.AddressTypeId;
                address.DoorNumber = updatedAddress.DoorNumber;
                address.Street = updatedAddress.Street;
                address.City = updatedAddress.City;
                address.State = updatedAddress.State;
                address.ZipCode = updatedAddress.ZipCode;
                _bookStoreDbContext.Update(address);
                await _bookStoreDbContext.SaveChangesAsync();
                return await GetAnAddress(userId, addressId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<AddressResponse>> DeleteAnAddress(int userId, int addressId)
        {
            try
            {
                Address address;
                address = await _bookStoreDbContext.Addresses
                .Where(a => a.AddressId == addressId).FirstOrDefaultAsync();
                if (address == null)
                {
                    throw new Exception("Address not found");
                }
                _bookStoreDbContext.Addresses.Remove(address);
                await _bookStoreDbContext.SaveChangesAsync();
                return await GetAllAddresses(userId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
