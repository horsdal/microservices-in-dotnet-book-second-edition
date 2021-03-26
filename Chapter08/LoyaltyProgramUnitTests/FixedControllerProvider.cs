namespace LoyaltyProgramUnitTests
{
  using System;
  using System.Linq;
  using System.Reflection;
  using Microsoft.AspNetCore.Mvc.Controllers;
  using Microsoft.Extensions.DependencyInjection;

  public class FixedControllerProvider : ControllerFeatureProvider
  {
    private readonly Type[] controllerTypes;

    public FixedControllerProvider(params Type[] controllerTypes) => this.controllerTypes = controllerTypes;

    protected override bool IsController(TypeInfo typeInfo) => this.controllerTypes.Contains(typeInfo);
  }

  public static class MvcBuilderExtensions
  {
    public static IMvcBuilder AddControllersByType(this IServiceCollection services, params Type[] controllerTypes) =>
      services
        .AddControllers()
        .ConfigureApplicationPartManager(mgr =>
        {
          mgr.FeatureProviders.Remove(mgr.FeatureProviders.First(f => f is ControllerFeatureProvider));
          mgr.FeatureProviders.Add(new FixedControllerProvider(controllerTypes));
        });

  }
}