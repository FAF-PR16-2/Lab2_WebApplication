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
            var database = client.GetDatabase("lab2_db");
            _usersCollection = database.GetCollection<User>("users");
            _tokenManager = jwtTokenManager;
            _passwordHasher = passwordHasher;

        }
        
        [AllowAnonymous]
        [HttpPost("sign-on")]
        public IActionResult Authenticate([FromBody] UserCredential credential)
        {
            var token = _tokenManager.Authenticate(credential.UserName, credential.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            return Ok(token);
        }
        
        [AllowAnonymous]
        [HttpPost("sign-up")]
        public IActionResult SignUp([FromBody] UserCredential credential)
        {
            
            
            var token = _tokenManager.Authenticate(credential.UserName, credential.Password);
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            return Ok(token);
        }
    }
}