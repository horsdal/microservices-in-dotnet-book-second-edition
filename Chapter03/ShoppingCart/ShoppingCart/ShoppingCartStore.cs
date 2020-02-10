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
    private static readonly Dictionary<int, ShoppingCart> database = new Dictionary<int, ShoppingCart>();

    public ShoppingCart Get(int userId) => 
      database.ContainsKey(userId) ? database[userId] : new ShoppingCart(userId);

    public void Save(ShoppingCart shoppingCart) => 
      database[shoppingCart.UserId] = shoppingCart;
  }
}