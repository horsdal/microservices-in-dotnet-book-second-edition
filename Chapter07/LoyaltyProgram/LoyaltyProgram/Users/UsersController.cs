namespace LoyaltyProgram.Users
{
  using System;
  using System.Collections.Generic;
  using Microsoft.AspNetCore.Mvc;

  [Route("/users")]
  public class UsersController : Controller
  {
    private static readonly IDictionary<int, LoyaltyProgramUser> RegisteredUsers = new Dictionary<int, LoyaltyProgramUser>();

    [HttpGet("fail")]
    public IActionResult Fail() => throw new NotImplementedException();

    [HttpGet("{userId:int}")]
    public ActionResult<LoyaltyProgramUser> GetUser(int userId) => 
      RegisteredUsers.ContainsKey(userId) 
        ? (ActionResult<LoyaltyProgramUser>) Ok(RegisteredUsers[userId])
        : NotFound();
    
    [HttpPost("")]
    public ActionResult<LoyaltyProgramUser> CreateUser([FromBody] LoyaltyProgramUser user)
    {
      throw new Exception();
      }

    [HttpPut("{userId:int}")]
    public LoyaltyProgramUser UpdateUser(
      int userId,
      [FromBody] LoyaltyProgramUser user)
      => RegisteredUsers[userId] = user;

    private LoyaltyProgramUser RegisterUser(LoyaltyProgramUser user)
    {
      var userId = RegisteredUsers.Count;
      user.Id = userId;
      return RegisteredUsers[userId] = user;
    }
  }
}