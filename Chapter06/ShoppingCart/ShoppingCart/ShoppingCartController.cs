namespace ShoppingCart.Shoppingcart
{
  using System.Threading.Tasks;
  using EventFeed;
  using Microsoft.AspNetCore.Mvc;
  using ShoppingCart;

  [Route("/shoppingcart")]
  public class ShoppingCartController : Controller
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
    public async Task<ShoppingCart> Get(int userId) => await this.shoppingCartStore.Get(userId);

    [HttpPost("{userId:int}/items")]
    public async Task<ShoppingCart> Post(int userId, [FromBody] int[] productIds)
    {
      var shoppingCart = await shoppingCartStore.Get(userId);
      var shoppingCartItems = await this.productCatalog.GetShoppingCartItems(productIds);
      await shoppingCart.AddItems(shoppingCartItems, this.eventStore);
      await this.shoppingCartStore.Save(shoppingCart);

      return shoppingCart;
    }

    [HttpDelete("{userid:int}/items")]
    public async Task<ShoppingCart> Delete(int userId, [FromBody] int[] productIds)
    {
      var shoppingCart = await this.shoppingCartStore.Get(userId);
      shoppingCart.RemoveItems(productIds, this.eventStore);
      await this.shoppingCartStore.Save(shoppingCart);
      return shoppingCart;
    }
  }
}
