namespace ProductCatalog
{
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.AspNetCore.Mvc;

  [Route("/products")]
  public class ProductCatalogController : Controller
  {
    private readonly IProductStore productStore;

    public ProductCatalogController(IProductStore productStore) => this.productStore = productStore;

    [HttpGet("")]
    [ResponseCache(Duration = 86400)]
    public IEnumerable<ProductCatalogProduct> Get()
    {
      var productIds = this.Request.Query["productIds"];
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
    public IEnumerable<ProductCatalogProduct> GetProductsByIds(IEnumerable<int> productIds)
    {
      return productIds.Select(id => new ProductCatalogProduct(id, "foo" + id, "bar", new Money()));
    }
  }

  public class ProductCatalogProduct
  {
    public ProductCatalogProduct(int productId, string productName, string description, Money price)
    {
      this.ProductId = productId.ToString();
      this.ProductName = productName;
      this.ProductDescription = description;
      this.Price = price;
    }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductDescription { get; private set; }
    public Money Price { get; private set; }
  }

  public class Money { }
}