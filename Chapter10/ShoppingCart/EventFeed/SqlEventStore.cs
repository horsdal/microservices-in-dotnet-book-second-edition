namespace ShoppingCart.EventFeed
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Dapper;

  public class SqlEventStore : IEventStore
  {
    private string connectionString =
      @"Data Source=localhost;Initial Catalog=ShoppingCart;
User Id=SA; Password=yourStrong(!)Password";

    private const string WriteEventSql =
      @"insert into EventStore(Name, OccurredAt, Content) values (@Name, @OccurredAt, @Content)";

    public async Task Raise(string eventName, object content)
    {
      var jsonContent = JsonSerializer.Serialize(content);
      await using var conn = new SqlConnection(this.connectionString);
      await conn.ExecuteAsync(
        WriteEventSql,
        new
        {
          Name = eventName,
          OccurredAt = DateTimeOffset.Now,
          Content = jsonContent
        });
    }

    private const string ReadEventsSql =
      @"select * from EventStore where ID >= @Start and ID <= @End";

    public async Task<IEnumerable<Event>> GetEvents(
      long firstEventSequenceNumber,
      long lastEventSequenceNumber)
    {
      await using var conn = new SqlConnection(this.connectionString);
      return await conn.QueryAsync<Event>(
          ReadEventsSql,
          new
          {
            Start = firstEventSequenceNumber,
            End = lastEventSequenceNumber
          });
    }
  }
}