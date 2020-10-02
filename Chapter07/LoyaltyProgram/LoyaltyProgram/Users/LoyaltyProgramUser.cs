namespace LoyaltyProgram.Users
{
  using System;

  public class LoyaltyProgramUser
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LoyaltyPoints { get; set; }
    public LoyaltyProgramSettings Settings { get; set; } = new LoyaltyProgramSettings();
  }
}