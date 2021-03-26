namespace ShoppingCart.ShoppingCart
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using EventFeed;

  public class ShoppingCart
  {
    private readonly HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();

    public int UserId { get; }
    public IEnumerable<ShoppingCartItem> Items => this.items;

    public ShoppingCart(int userId) => this.UserId = userId;

    public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
    {
      foreach (var item in shoppingCartItems)
        if (this.items.Add(item))
          eventStore.Raise("ShoppingCartItemAdded", new {this.UserId, item });
    }

    public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore) =>
      this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
  }

  public record ShoppingCartItem(
    int ProductCatalogueId,
    string ProductName,
    string Description,
    Money Price)
  {
    public virtual bool Equals(ShoppingCartItem? obj) =>
      obj != null && this.ProductCatalogueId.Equals(obj.ProductCatalogueId);

    public override int GetHashCode() => this.ProductCatalogueId.GetHashCode();
  }

  public record Money(string Currency, decimal Amount);
}