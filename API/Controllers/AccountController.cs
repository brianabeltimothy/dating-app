using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        } 

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.username)) return BadRequest("username is taken");

            //generate unique key
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)), //hash the password with unique key
                PasswordSalt = hmac.Key //store the unique key
            };

            _context.Users.Add(user); //a new user to the in-memory representation of the "Users" table
            await _context.SaveChangesAsync(); //commits the changes to the database

            return new UserDto
            {
                username = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower()); //x represent every record in the database
            //AnyAsync is used to check if any elements in a sequence match a specified condition
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => 
            x.UserName == loginDto.username);

            if(user == null) return Unauthorized();

            using var hmac = new HMACSHA512(user.PasswordSalt); //get the password's salt
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password)); // get the hashed password.

            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }

            return new UserDto
            {
                username = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }
    }
}