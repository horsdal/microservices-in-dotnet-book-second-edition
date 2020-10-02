namespace ShoppingCart.EventFeed
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Threading.Tasks;
  using Dapper;
  using Newtonsoft.Json;

  public class SqlEventStore : IEventStore
  {
    private string connectionString =
      @"Data Source=localhost;Initial Catalog=ShoppingCart;
User Id=SA; Password=yourStrong(!)Password";

    private const string writeEventSql =
      @"insert into EventStore(Name, OccurredAt, Content) values (@Name, @OccurredAt, @Content)";

    public async Task Raise(string eventName, object content)
    {
      var jsonContent = JsonConvert.SerializeObject(content);
      await using var conn = new SqlConnection(this.connectionString);
      await conn.ExecuteAsync(
        writeEventSql,
        new
        {
          Name = eventName,
          OccurredAt = DateTimeOffset.Now,
          Content = jsonContent
        });
    }

    private const string readEventsSql =
      @"select * from EventStore where ID >= @Start and ID <= @End";

    public async Task<IEnumerable<Event>> GetEvents(
      long firstEventSequenceNumber,
      long lastEventSequenceNumber)
    {
      await using var conn = new SqlConnection(this.connectionString);
      return await conn.QueryAsync<Event>(
          readEventsSql,
          new
          {
            Start = firstEventSequenceNumber,
            End = lastEventSequenceNumber
          });
    }
  }
}