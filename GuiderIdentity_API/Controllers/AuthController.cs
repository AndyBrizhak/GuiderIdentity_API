﻿using GuiderIdentity_API.Data;
using GuiderIdentity_API.Migrations;
using GuiderIdentity_API.Models;
using GuiderIdentity_API.Models.DTO;
using GuiderIdentity_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GuiderIdentity_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        public AuthController(ApplicationDbContext db, 
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>
                ("ApiSettings:Secret"); 
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            ApplicationUser userFromDb = 
                _db.ApplicationUsers
                .FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

            if (userFromDb != null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exist");
                return BadRequest(_response);
            }

            ApplicationUser newUser = new()
            {
                UserName = model.UserName,
                Email = model.UserName,
                NormalizedUserName = model.UserName.ToUpper(),
                Name = model.Name
            };

            try
            {
                var result =
                await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(Utility.SD.Role_Admin)
                        .GetAwaiter().GetResult())
                    {
                        //create role on database
                        await _roleManager
                            .CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager
                            .CreateAsync(new IdentityRole(SD.Role_Customer));
                    }

                    if (model.Role.ToLower() == SD.Role_Admin)
                    {
                        await _userManager
                            .AddToRoleAsync(newUser, SD.Role_Admin);
                    }
                    else
                    {
                        await _userManager
                            .AddToRoleAsync(newUser, SD.Role_Customer);
                    }
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
            }
            catch (Exception)
            {

                
            }
            
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while rgistration");
            return BadRequest(_response);
        }

    }
}
