# Use .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# Use .NET 9.0 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["PersonalHomepage.csproj", "./"]
RUN dotnet restore "PersonalHomepage.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src"
RUN dotnet build "PersonalHomepage.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PersonalHomepage.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Create uploads directory with proper permissions
RUN mkdir -p /app/wwwroot/uploads/profiles && \
    chown -R appuser:appuser /app

# Copy published application
COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PersonalHomepage.dll"]