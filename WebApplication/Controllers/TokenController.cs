using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenManager _tokenManager;
        private IMongoCollection<User> _usersCollection;
        private IPasswordHasher _passwordHasher;
        

        public TokenController(IJwtTokenManager jwtTokenManager, IPasswordHasher passwordHasher, IMongoClient client)
        {
            var database = client.GetDatabase("applicationDB");
            _usersCollection = database.GetCollection<User>("users");
            _tokenManager = jwtTokenManager;
            _passwordHasher = passwordHasher;

        }
        
        [AllowAnonymous]
        [HttpPost("sign-on")]
        public IActionResult Authenticate([FromBody] UserCredential credential)
        {
            if (CheckIfAccountExists(credential.UserName, credential.Password))
                return Unauthorized();
            var token = _tokenManager.Authenticate(credential.UserName);//, credential.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            return Ok(token);
        }
        
        [AllowAnonymous]
        [HttpPost("sign-up")]
        public IActionResult SignUp([FromBody] UserCredential credential)
        {
            if (CheckIfNameOrEmailIsFree(credential.UserName, credential.Email) != UsernameStatus.Free)
                return Unauthorized();
            var token = _tokenManager.Authenticate(credential.UserName);//, credential.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var newDBUser = new User
            {
                UserName = credential.UserName,
                Email = credential.Email,
                Password = _passwordHasher.HashString(credential.Password)
            };

            _usersCollection.InsertOne(newDBUser);
            
            return Ok(token);
        }

        private enum UsernameStatus
        {
            ThisUsernameAlreadyExists,
            ThisEmailAlreadyExists,
            Free
        }

        private UsernameStatus CheckIfNameOrEmailIsFree(string userName, string email)
        {
            List<User> userNames = _usersCollection.Find(user => user.UserName == userName)
                .ToList();

            if (userNames.Count > 0)
                return UsernameStatus.ThisUsernameAlreadyExists;
            
            List<User> emails = _usersCollection.Find(user => user.Email == email)
                .ToList();
            
            if (emails.Count > 0)
                return UsernameStatus.ThisEmailAlreadyExists;
            
            return UsernameStatus.Free;
        }

        private bool CheckIfAccountExists(string userName, string password)
        {
            List<User> users = 
                _usersCollection.Find(user => user.UserName == userName
                                              && _passwordHasher.VerifyPassword(password, user.Password))
                .ToList();

            if (users.Count < 1)
                return false;
            
            if (users.Count > 1)
                Console.WriteLine("wtf?");

            return true;
        }
            
    }
}