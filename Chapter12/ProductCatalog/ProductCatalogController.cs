namespace ProductCatalog
{
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.AspNetCore.Mvc;

  [Route("/products")]
  public class ProductCatalogController
  {
    private readonly IProductStore productStore;

    public ProductCatalogController(IProductStore productStore) => this.productStore = productStore;

    [HttpGet("")]
    [ResponseCache(Duration = 86400)]
    public IEnumerable<ProductCatalogProduct> Get([FromQuery] string productIds)
    {
      var products = this.productStore.GetProductsByIds(ParseProductIdsFromQueryString(productIds));
      return products;
    }

    private static IEnumerable<int> ParseProductIdsFromQueryString(string productIdsString) => productIdsString.Split(',').Select(s => s.Replace("[", "").Replace("]", "")).Select(int.Parse);
  }

  public interface IProductStore
  {
    IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds);
  }

  public class ProductStore : IProductStore
  {
    public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds) =>
      productIds.Select(id => new ProductCatalogProduct(id, "Product " + id, "lorum ipsum", new Money()));
  }

  public record ProductCatalogProduct(int ProductId, string ProductName, string Description, Money Price);

  public record Money();
}