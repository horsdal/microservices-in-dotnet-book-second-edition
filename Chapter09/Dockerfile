FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ShoppingCart/ShoppingCart.csproj"
WORKDIR "/src/ShoppingCart"
RUN dotnet build "ShoppingCart.csproj" -c Release -o /api/build

FROM build AS publish
WORKDIR "/src/ShoppingCart"
RUN dotnet publish "ShoppingCart.csproj" -c Release -o /api/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /api/publish ./api
ENTRYPOINT dotnet api/LoyaltyProgram.dll
