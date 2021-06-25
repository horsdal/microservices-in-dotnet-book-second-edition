namespace ApiGateway.ProductList
{
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;

  public class ProductListController : Controller
  {
    private readonly HttpClient productCatalogClient;
    private readonly HttpClient shoppingCartClient;

    public ProductListController(IHttpClientFactory httpClientFactory)
    {
      this.productCatalogClient = httpClientFactory.CreateClient("ProductCatalogClient");
      this.shoppingCartClient = httpClientFactory.CreateClient("ShoppingCartClient");
    }

    [HttpGet("/productlist")]
    public async Task<IActionResult> ProductList([FromQuery] int userId)
    {
      var products = await GetProductsFromCatalog();
      var cartProducts = await GetProductsFromCart(userId);
#if false
      var products = new[]
      {
        new Product(1, "T-shirt", "Really nice t-shirt"),
        new Product(2, "Hoodie", "The coolest hoodie ever"),
        new Product(3, "Jeans", "Perfect jeans"),
      };
#endif
      return View(new ProductListViewModel(
        products,
        cartProducts
      ));
    }

    [HttpPost("/shoppingcart/{userId}")]
    public async Task<OkResult> AddToCart(int userId, [FromBody] int productId)
    {
      var response = await this.shoppingCartClient.PostAsJsonAsync($"/shoppingcart/{userId}/items", new[] {productId});
      response.EnsureSuccessStatusCode();
      return Ok();
    }

    [HttpDelete("/shoppingcart/{userId}")]
    public async Task<OkResult> RemoveFromCart(int userId, [FromBody] int productId)
    {
      var request = new HttpRequestMessage(HttpMethod.Delete, $"/shoppingcart/{userId}/items");
      request.Content = new StringContent(JsonSerializer.Serialize(new[] {productId}));
      var response = await this.shoppingCartClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
      return Ok();
    }

    private async Task<Product[]> GetProductsFromCart(int userId)
    {
      var response = await this.shoppingCartClient.GetAsync($"/shoppingcart/{userId}");
      response.EnsureSuccessStatusCode();
      var content = await response.Content.ReadAsStreamAsync();
      var cart =
        await JsonSerializer.DeserializeAsync<ShoppingCart>(content,
          new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
      return cart.Items;
    }

    private async Task<Product[]> GetProductsFromCatalog()
    {
      var response = await this.productCatalogClient.GetAsync("/products?productIds=1,2,3,4");
      response.EnsureSuccessStatusCode();
      var content = await response.Content.ReadAsStreamAsync();
      var products =
        await JsonSerializer.DeserializeAsync<Product[]>(content,
          new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
      return products;
    }
  }

  public record Product(int ProductId, string ProductName, string Description);

  public record ShoppingCart(int UserId, Product[] Items);

  public record ProductListViewModel(Product[] Products, Product[] CartProducts);

}