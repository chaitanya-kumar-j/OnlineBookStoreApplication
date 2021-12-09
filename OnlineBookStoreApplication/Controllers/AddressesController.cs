using BusinessLogicLayer.Interfaces;
using CommonLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBookStoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        public readonly IBookStoreLogic _bookStoreLogic;
        public readonly IConfiguration _configuration;
        public AddressesController(IBookStoreLogic bookStoreLogic, IConfiguration configuration)
        {
            this._bookStoreLogic = bookStoreLogic;
            this._configuration = configuration;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllAddresses()
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<AddressResponse> addresses = await this._bookStoreLogic.GetAllAddresses(userId);
                return this.Ok(new { Success = true, Message = "Get all Addresses of user is successful", Data = addresses });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpGet("{addressId:int}")]
        public async Task<ActionResult> GetAnAddress(int addressId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                AddressResponse address = await this._bookStoreLogic.GetAnAddress(userId, addressId);
                return this.Ok(new { Success = true, Message = "Get an Address of user is successful", Data = address });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpGet("AddressType/{addressTypeId:int}")]
        public async Task<ActionResult> GetAllAddressesOfAType(int addressTypeId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<AddressResponse> addresses = await this._bookStoreLogic.GetAllAddressesOfAType(userId, addressTypeId);
                return this.Ok(new { Success = true, Message = "Get user's all Addresses of an address type is successful", Data = addresses });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddAnAddress(AddressInput inputAddress)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                AddressResponse address = await this._bookStoreLogic.AddAnAddress(userId, inputAddress);
                return this.Ok(new { Success = true, Message = "Adding an address of user is successful", Data = address });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpPut("{addressId:int}")]
        public async Task<ActionResult> UpdateAnAddress(int addressId, AddressInput updatedAddress)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                AddressResponse address = await this._bookStoreLogic.UpdateAnAddress(userId, addressId, updatedAddress);
                return this.Ok(new { Success = true, Message = "Update an address of user is successful", Data = address });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("{addressId:int}")]
        public async Task<ActionResult> DeleteAnAddress(int addressId)
        {
            var currentUser = HttpContext.User;
            int userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
            try
            {
                List<AddressResponse> addresses = await this._bookStoreLogic.DeleteAnAddress(userId, addressId);
                return this.Ok(new { Success = true, Message = "Delete an address of user is successful", Data = addresses });
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }
    }
}
