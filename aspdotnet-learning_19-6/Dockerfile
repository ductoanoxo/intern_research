# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY EcommerceApi/EcommerceApi.csproj ./EcommerceApi/
RUN dotnet restore EcommerceApi/EcommerceApi.csproj
COPY EcommerceApi/ ./EcommerceApi/
RUN dotnet publish EcommerceApi/EcommerceApi.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# Copy the SQLite database file
COPY ecommerce.db /app/ecommerce.db
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
# Override connection string for containerized environment
ENV ConnectionStrings__DefaultConnection="Data Source=ecommerce.db"
ENTRYPOINT ["dotnet", "EcommerceApi.dll"]
