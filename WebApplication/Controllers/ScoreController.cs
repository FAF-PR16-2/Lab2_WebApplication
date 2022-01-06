using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : Controller
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Score> _scoresCollection;

        public ScoreController(IMongoClient client)
        {
            var database = client.GetDatabase("applicationDB");
            _usersCollection = database.GetCollection<User>("users");
            _scoresCollection = database.GetCollection<Score>("scores");
        }
        
        [HttpPost("score")]
        public IActionResult SetPlayerScore([FromBody] ScoreInfo scoreInfo)
        {
            var username = Request.HttpContext.User.Claims.FirstOrDefault(x => 
                x.Type == ClaimTypes.NameIdentifier)?.Value;

            User user = _usersCollection.Find(user => user.UserName == username)
                .ToList().First();
            
            
            Score newScore = new Score
            {
                UserId = user.Id,
                score = scoreInfo.Score
            };
            
            _scoresCollection.InsertOne(newScore);

            return Ok();
        }
        
        [HttpGet("score")]
        public IActionResult GetTopPlayersScore()
        {
            var result = (from scores in _scoresCollection.AsQueryable()
                join users in _usersCollection.AsQueryable()
                    on scores.UserId
                    equals users.Id
                orderby scores.score descending 
                    
                select new ScoreInfo
                {
                    NickName = users.UserName, 
                    Score = scores.score
                })
                .Take(10);
            
            
            
            return Ok(result);
        }

    }
}