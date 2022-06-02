using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NationalParksAPI.Data;
using NationalParksAPI.Models;
using NationalParksAPI.Repository.IRepository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace NationalParksAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly AppSettings appSettings;
        public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            this.db = db;
            this.appSettings = appSettings.Value;
        }

        public AuthenticationModel Authenticate(string username, string password)
        {
            var user = db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            //user not found
            if (user == null)
            {
                return null;
            }

            //if user was found, generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";
            return user;
        }

        public bool IsUniqueUser(string username)
        {
            var user = db.Users.SingleOrDefault(x => x.Username == username);
            //return null if user is not found
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public AuthenticationModel Register(string username, string password)
        {
            AuthenticationModel userObj = new AuthenticationModel()
            {
                Username = username,
                Password = password,
                Role="Admin"
            };
            db.Users.Add(userObj);
            db.SaveChanges();
            userObj.Password = "";
            return userObj;
        }
    }
}
