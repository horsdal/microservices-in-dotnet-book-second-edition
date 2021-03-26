namespace ShoppingCart.ShoppingCart
{
  using System.Threading.Tasks;
  using EventFeed;
  using Microsoft.AspNetCore.Mvc;

  [Route("/shoppingcart")]
  public class ShoppingCartController
  {
    private readonly IShoppingCartStore shoppingCartStore;
    private readonly IProductCatalogClient productCatalog;
    private readonly IEventStore eventStore;

    public ShoppingCartController(
      IShoppingCartStore shoppingCartStore,
      IProductCatalogClient productCatalog,
      IEventStore eventStore)
    {
      this.shoppingCartStore = shoppingCartStore;
      this.productCatalog = productCatalog;
      this.eventStore = eventStore;
    }

    [HttpGet("{userId:int}")]
    public ShoppingCart Get(int userId) => this.shoppingCartStore.Get(userId);

    [HttpPost("{userId:int}/items")]
    public async Task<ShoppingCart> Post(int userId, [FromBody] int[] productIds)
    {
      var shoppingCart = shoppingCartStore.Get(userId);
      var shoppingCartItems = await this.productCatalog.GetShoppingCartItems(productIds);
      shoppingCart.AddItems(shoppingCartItems, eventStore);
      this.shoppingCartStore.Save(shoppingCart);

      return shoppingCart;
    }

    [HttpDelete("{userid:int}/items")]
    public ShoppingCart Delete(int userId, [FromBody] int[] productIds)
    {
      var shoppingCart = this.shoppingCartStore.Get(userId);
      shoppingCart.RemoveItems(productIds, this.eventStore);
      this.shoppingCartStore.Save(shoppingCart);
      return shoppingCart;
    }
  }
}