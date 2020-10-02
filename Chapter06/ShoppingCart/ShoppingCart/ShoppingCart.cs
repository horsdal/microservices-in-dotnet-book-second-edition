namespace ShoppingCart.ShoppingCart
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using EventFeed;

  public class ShoppingCart
  {
    private readonly HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();

    public int? Id { get; }
    public int UserId { get; }
    public IEnumerable<ShoppingCartItem> Items => this.items;

    public ShoppingCart(int userId) => this.UserId = userId;

    public ShoppingCart(int? id, int userId, IEnumerable<ShoppingCartItem> items)
    {
      this.Id = id;
      this.UserId = userId;
      foreach (var item in items)
        this.items.Add(item);
    }

    public async Task AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
      foreach (var item in shoppingCartItems)
        if (this.items.Add(item))
          await eventStore.Raise("ShoppingCartItemAdded", new {this.UserId, item });
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore) =>
      this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogId));
  }

  public class ShoppingCartItem
  {
    public int ProductCatalogId { get; }
    public string ProductName { get; }
    public string Description { get; }
    public Money Price { get; }

    public ShoppingCartItem(
      int productCatalogId,
      string productName,
      string description,
      Money price)
    {
      this.ProductCatalogId = productCatalogId;
      this.ProductName = productName;
      this.Description = description;
      this.Price = price;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      var that = obj as ShoppingCartItem;
      return this.ProductCatalogId.Equals(that.ProductCatalogId);
    }

    public override int GetHashCode()
    {
      return this.ProductCatalogId.GetHashCode();
    }
  }

  public class Money
  {
    public string Currency { get; }
    public decimal Amount { get; }

    public Money(string currency, decimal amount)
    {
      this.Currency = currency;
      this.Amount = amount;
    }
  }
}