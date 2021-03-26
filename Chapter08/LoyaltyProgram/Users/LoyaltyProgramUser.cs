namespace LoyaltyProgram.Users
{
  using System;

  public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);

  public record LoyaltyProgramSettings()
  {
    public LoyaltyProgramSettings(params string[] interests) : this()
    {
      this.Interests = interests;
    }

    public string[] Interests { get;  } = Array.Empty<string>();
  }

}