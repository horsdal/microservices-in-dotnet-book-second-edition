namespace ShoppingCart
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;

  public interface ICache
  {
    void Add(string key, object value, TimeSpan ttl);
    object? Get(string key);
  }

  public class Cache : ICache
  {
    private static readonly IDictionary<string, (DateTimeOffset, object)> cache = new ConcurrentDictionary<string, (DateTimeOffset, object)>();

    public void Add(string key, object value, TimeSpan ttl) =>
      cache[key] = (DateTimeOffset.UtcNow.Add(ttl), value);

    public object? Get(string productsResource)
    {
      if (cache.TryGetValue(productsResource, out var value)
          && value.Item1 > DateTimeOffset.UtcNow)
        return value;
      cache.Remove(productsResource);
      return null;
    }
  }
}