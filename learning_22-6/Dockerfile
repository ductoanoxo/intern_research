# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching restore layer
COPY CleanArchitectureDemo.sln ./
COPY src/CleanArchitectureDemo.Domain/CleanArchitectureDemo.Domain.csproj src/CleanArchitectureDemo.Domain/
COPY src/CleanArchitectureDemo.Application/CleanArchitectureDemo.Application.csproj src/CleanArchitectureDemo.Application/
COPY src/CleanArchitectureDemo.Infrastructure/CleanArchitectureDemo.Infrastructure.csproj src/CleanArchitectureDemo.Infrastructure/
COPY src/CleanArchitectureDemo.Api/CleanArchitectureDemo.Api.csproj src/CleanArchitectureDemo.Api/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ src/

# Build and Publish
RUN dotnet publish src/CleanArchitectureDemo.Api/CleanArchitectureDemo.Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection="Data Source=bookstore.db"

EXPOSE 8080
ENTRYPOINT ["dotnet", "CleanArchitectureDemo.Api.dll"]
