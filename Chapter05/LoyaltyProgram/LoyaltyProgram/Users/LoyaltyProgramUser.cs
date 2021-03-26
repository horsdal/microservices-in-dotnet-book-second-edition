namespace LoyaltyProgram.Users
{
  using System;

  public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);

  public record LoyaltyProgramSettings()
  {
    public LoyaltyProgramSettings(string[] interests) : this()
    {
      this.Interests = interests;
    }

    public string[] Interests { get; init; } = Array.Empty<string>();
  }
}