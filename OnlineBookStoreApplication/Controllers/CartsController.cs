namespace OnlineBookStoreApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BusinessLogicLayer.Interfaces;
    using CommonLayer.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using RepositoryLayer.Entities;

    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        public readonly IBookStoreLogic _bookStoreLogic;
        public readonly IConfiguration _configuration;
        public CartsController(IBookStoreLogic bookStoreLogic, IConfiguration configuration)
        {
            this._bookStoreLogic = bookStoreLogic;
            this._configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllBooksInCart()
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<CartResponse> cart = await this._bookStoreLogic.GetAllBooksInCart(userId);
                return this.Ok(new { Success = true, Message = "Get all books in cart is successful", Data = cart });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("{bookId:int}")]
        public async Task<ActionResult> GetABookInCart(int bookId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                CartResponse cart = await this._bookStoreLogic.GetABookInCart(userId, bookId);
                return this.Ok(new { Success = true, Message = "Get a book in cart is successful", Data = cart });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("{bookId:int}")]
        public async Task<ActionResult> AddABookToCart(int bookId, int numberOfBooks)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                CartResponse cart = await this._bookStoreLogic.AddABookToCart(userId, bookId, numberOfBooks);
                return this.Ok(new { Success = true, Message = "Add a book to cart is successful", Data = cart });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("{bookId:int}")]
        public async Task<ActionResult> UpdateABookInCart(int bookId, int numberOfBooks)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                CartResponse cart = await this._bookStoreLogic.UpdateABookInCart(userId, bookId, numberOfBooks);
                return this.Ok(new { Success = true, Message = "Update a book in cart is successful", Data = cart });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("{bookId:int}")]
        public async Task<ActionResult> DeleteABookInCart(int bookId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<CartResponse> carts = await this._bookStoreLogic.DeleteABookInCart(userId, bookId);
                return this.Ok(new { Success = true, Message = "Update a book in cart is successful", Data = carts });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("{bookId:int}/MoveToWishList")]
        public async Task<ActionResult> MoveABookToWishList(int bookId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<CartResponse> carts = await this._bookStoreLogic.MoveABookToWishList(userId, bookId);
                return this.Ok(new { Success = true, Message = "Move a book in cart to wishlist is successful", Data = carts });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }
    }
}
