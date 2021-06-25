namespace LoyaltyProgram.Users
{
  using System;
  using System.Collections.Generic;
  using System.IdentityModel.Tokens.Jwt;
  using System.Linq;
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Route("/users")]
  public class UsersController : ControllerBase
  {
    private static readonly IDictionary<int, LoyaltyProgramUser> RegisteredUsers = new Dictionary<int, LoyaltyProgramUser>();

    [HttpGet("{userId:int}")]
    public ActionResult<LoyaltyProgramUser> GetUser(int userId) => 
      RegisteredUsers.ContainsKey(userId) 
        ? (ActionResult<LoyaltyProgramUser>) Ok(RegisteredUsers[userId])
        : NotFound();
    
    [HttpPost("")]
    public ActionResult<LoyaltyProgramUser> CreateUser([FromBody] LoyaltyProgramUser user) =>
      Created(new Uri($"/users/{RegisterUser(user).Id}", UriKind.Relative), RegisterUser(user));

    [HttpPut("{userId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public ActionResult<LoyaltyProgramUser> UpdateUser(
      int userId,
      [FromBody] LoyaltyProgramUser user)
    {
      var hasUserId = int.TryParse(
        this.User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value,
        out var userIdFromToken);
      if (!hasUserId || userId != userIdFromToken)
        return Unauthorized();

      return RegisteredUsers[userId] = user;
    }

    private LoyaltyProgramUser RegisterUser(LoyaltyProgramUser user)
    {
      var userId = RegisteredUsers.Count;
      return RegisteredUsers[userId] = user with { Id = userId};
    }
  }
}