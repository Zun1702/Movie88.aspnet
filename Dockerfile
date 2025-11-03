# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files only (skip solution to avoid docker-compose.dcproj issue)
COPY Movie88.Domain/Movie88.Domain.csproj ./Movie88.Domain/
COPY Movie88.Application/Movie88.Application.csproj ./Movie88.Application/
COPY Movie88.Infrastructure/Movie88.Infrastructure.csproj ./Movie88.Infrastructure/
COPY Movie88.WebApi/Movie88.WebApi.csproj ./Movie88.WebApi/

# Restore dependencies for each project
RUN dotnet restore Movie88.Domain/Movie88.Domain.csproj
RUN dotnet restore Movie88.Application/Movie88.Application.csproj
RUN dotnet restore Movie88.Infrastructure/Movie88.Infrastructure.csproj
RUN dotnet restore Movie88.WebApi/Movie88.WebApi.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/Movie88.WebApi
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port (Railway will set PORT environment variable)
EXPOSE 8080

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "Movie88.WebApi.dll"]
