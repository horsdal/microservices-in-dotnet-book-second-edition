namespace ShoppingCart.ShoppingCart
{
  using System.Collections.Generic;

  public interface IShoppingCartStore
  {
    ShoppingCart Get(int userId);
    void Save(ShoppingCart shoppingCart);
  }

  public class ShoppingCartStore : IShoppingCartStore
  {
    private static readonly Dictionary<int, ShoppingCart> Database = new Dictionary<int, ShoppingCart>();

    public ShoppingCart Get(int userId) =>
      Database.ContainsKey(userId) ? Database[userId] : new ShoppingCart(userId);

    public void Save(ShoppingCart shoppingCart) =>
      Database[shoppingCart.UserId] = shoppingCart;
  }
}