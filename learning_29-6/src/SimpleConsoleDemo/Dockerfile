FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY SimpleConsoleDemo.csproj ./
RUN dotnet restore

# Copy source and build
COPY Program.cs ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "SimpleConsoleDemo.dll"]
