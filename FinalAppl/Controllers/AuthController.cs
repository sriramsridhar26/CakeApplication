using CakeApplication.Data;
using CakeApplication.DTO;
using CakeApplication.Model;
using CakeApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CakeApplication.Controllers
{
    public class AuthController : ControllerBase
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("/Register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register(UserRegisterDto user)
        {
            User user1 = new User();
            ServiceResponse<string> response = new ServiceResponse<string>();
            if (await UserExists(user.emailId))
            {
                response.Success = false;
                response.Message = "User already exists";
                return BadRequest(response);
            }
            else
            {
                CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user1.PasswordHash = passwordHash;
                user1.PasswordSalt = passwordSalt;
                user1.emailId = user.emailId;
                user1.customerName = user.customerName;
                user1.Address = user.Address;
                user1.MobileNo = user.MobileNo;
                _context.users.Add(user1);
                await _context.SaveChangesAsync();
                response.Data = user.emailId;
                return Ok(response);
            }

        }

        [HttpPost("/Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] UserLoginDto userd)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.users.FirstOrDefaultAsync(u => u.emailId.ToLower().Equals(userd.emailId.ToLower()));
            if(user == null)
            {
                response.Success=false;
                response.Message = "user not found";
                return BadRequest(response);
            }
            else if(!VerifyPasswordHash(userd.password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect credentials";
                return BadRequest(response);

            }
            else
            {
                response.Data = CreateToken(user);
                return Ok(response);
            }
        }



        //Methods for endpoints
        public async Task<bool> UserExists(string emailId)
        {
            if (await _context.users.AnyAsync(u => u.emailId.ToLower() == emailId.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.emailId)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
