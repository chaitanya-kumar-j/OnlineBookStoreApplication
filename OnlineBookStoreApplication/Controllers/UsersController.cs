namespace OnlineBookStoreApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessLogicLayer.Interfaces;
    using CommonLayer.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using RepositoryLayer.Entities;
    using RepositoryLayer.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IBookStoreLogic _bookStoreLogic;
        public readonly BookStoreDbContext _bookStoreDbContext;
        public readonly IConfiguration _configuration;
        public UsersController(IBookStoreLogic bookStoreLogic, IConfiguration configuration, BookStoreDbContext bookStoreDbContext)
        {
            this._bookStoreLogic = bookStoreLogic;
            this._configuration = configuration;
            this._bookStoreDbContext = bookStoreDbContext;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> UserRegistration(User user)
        {
            try
            {
                User usersData = await this._bookStoreLogic.UserRegistration(user);
                return this.Ok(new { Success = true, Message = "User registration is successful", Data = usersData });
            }
            catch(Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }

        [HttpPost]
        [Route("Login")]

        public async Task<ActionResult> UserLogin(UserLogin userLogin)
        {
            try
            {
                User usersData = await this._bookStoreLogic.UserLogin(userLogin);
                string token = new JwtService(this._configuration).GenerateSecurityToken(usersData);
                return this.Ok(new { Success = true, Message = "User Login is successful", Data = usersData , Token = token});
            }
            catch (Exception e)
            {
                return this.BadRequest(new { Success = false, Message = e.Message });
            }
        }
    }
}
