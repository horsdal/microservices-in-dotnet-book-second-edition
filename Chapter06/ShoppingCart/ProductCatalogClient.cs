namespace ShoppingCart
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  using Newtonsoft.Json;
  using ShoppingCart;

  public interface IProductCatalogClient
  {
    Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogueIds);
  }

  public class ProductCatalogClient : IProductCatalogClient
  {
    private readonly HttpClient client;
    private readonly ICache cache;
    private static string productCatalogueBaseUrl =  @"https://git.io/JeHiE";
    private static string getProductPathTemplate = "?productIds=[{0}]";

    public ProductCatalogClient(HttpClient client, ICache cache)
    {
      client.BaseAddress = new Uri(productCatalogueBaseUrl);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      this.client = client;
      this.cache = cache;
    }
    
    public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds)
    {
      var response = await RequestProductFromProductCatalog(productCatalogIds);
      return await ConvertToShoppingCartItems(response);
    }

    private async Task<HttpResponseMessage> RequestProductFromProductCatalog(int[] productCatalogIds)
    {
      var productsResource = string.Format(getProductPathTemplate, string.Join(",", productCatalogIds));
      var response = this.cache.Get(productsResource) as HttpResponseMessage;
      if (response is null)
      {
        response = await this.client.GetAsync(productsResource);
        AddToCache(productsResource, response);
      }

      return response;
    }

    private void AddToCache(string resource, HttpResponseMessage response)
    {
      var cacheHeader = response.Headers.FirstOrDefault(h => h.Key == "cache-control");
      if (!string.IsNullOrEmpty(cacheHeader.Key)
          && CacheControlHeaderValue.TryParse(cacheHeader.Value.ToString(), out var cacheControl)
          && cacheControl.MaxAge.HasValue)
        this.cache.Add(resource, response, cacheControl.MaxAge.Value);
    }

    private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
    {
      response.EnsureSuccessStatusCode();
      var products = JsonConvert.DeserializeObject<List<ProductCatalogProduct>>(await response.Content.ReadAsStringAsync());
      return products
        .Select(p => 
          new ShoppingCartItem(
            int.Parse(p.ProductId),
            p.ProductName,
            p.ProductDescription,
            p.Price
        ));
    }

    private class ProductCatalogProduct
    {
      public string ProductId { get; set; }
      public string ProductName { get; set; }
      public string ProductDescription { get; set; }
      public Money Price { get; set; }
    }
  }
}