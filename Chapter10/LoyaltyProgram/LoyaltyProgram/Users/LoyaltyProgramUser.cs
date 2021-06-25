namespace LoyaltyProgram.Users
{
  public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);

  public record LoyaltyProgramSettings(string[] Interests);

}