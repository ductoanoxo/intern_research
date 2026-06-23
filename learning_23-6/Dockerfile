# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching restore layer
COPY DddCqrsJwtDemo.sln ./
COPY src/DddCqrsJwtDemo.Domain/DddCqrsJwtDemo.Domain.csproj src/DddCqrsJwtDemo.Domain/
COPY src/DddCqrsJwtDemo.Application/DddCqrsJwtDemo.Application.csproj src/DddCqrsJwtDemo.Application/
COPY src/DddCqrsJwtDemo.Infrastructure/DddCqrsJwtDemo.Infrastructure.csproj src/DddCqrsJwtDemo.Infrastructure/
COPY src/DddCqrsJwtDemo.Api/DddCqrsJwtDemo.Api.csproj src/DddCqrsJwtDemo.Api/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ src/

# Build and Publish
RUN dotnet publish src/DddCqrsJwtDemo.Api/DddCqrsJwtDemo.Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection="Data Source=bookstore.db"

EXPOSE 8080
ENTRYPOINT ["dotnet", "DddCqrsJwtDemo.Api.dll"]
