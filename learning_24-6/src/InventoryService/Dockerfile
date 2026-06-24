FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore
COPY MicroservicesDemo.sln ./
COPY src/InventoryService/InventoryService.csproj src/InventoryService/
RUN dotnet restore src/InventoryService/InventoryService.csproj

# Copy other source code
COPY src/InventoryService/ src/InventoryService/
RUN dotnet publish src/InventoryService/InventoryService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8082
EXPOSE 8082
EXPOSE 8083
ENTRYPOINT ["dotnet", "InventoryService.dll"]
