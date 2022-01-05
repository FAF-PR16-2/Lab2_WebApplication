using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IUserManager _userManager;
        
        public ScoreController(IMongoClient client, IUserManager userManager)
        {
            var database = client.GetDatabase("applicationDB");
            _usersCollection = database.GetCollection<User>("users");
            _scoresCollection = database.GetCollection<Score>("scores");

            _userManager = userManager;
        }
        
        [HttpPost("score")]
        public IActionResult SetPlayerScore([FromBody] ScoreInfo scoreInfo)
        {
            var username = Request.HttpContext.User.Claims.FirstOrDefault(x => 
                x.Type == ClaimTypes.NameIdentifier)?.Value;

            List<User> users = _usersCollection.Find(user => user.UserName == username)
                .ToList();
            
            
            Score newScore = new Score
            {
                UserId = users[0].Id,
                nickName = scoreInfo.NickName,
                score = scoreInfo.Score
            };
            
            _scoresCollection.InsertOne(newScore);

            return Ok();
        }
        
        [HttpGet("score")]
        public IActionResult GetTopPlayersScore()
        {
            // Score newScore = new Score
            // {
            //     UserId = _userManager.CurrentUser.Id,
            //     nickName = scoreInfo.NickName,
            //     score = scoreInfo.Score
            // };
            
            List<Score> top10scores = _scoresCollection
                .Find(FilterDefinition<Score>.Empty).Limit(10).Sort(new BsonDocument("score", -1))
                .ToList();

            return Ok(top10scores);
        }

        // private ScoreInfo[] GetPlayerScores()
        // {
        //     
        // }
        
    }
}