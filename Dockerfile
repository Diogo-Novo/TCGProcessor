# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY TCGProcessor.csproj ./
RUN dotnet restore TCGProcessor.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish TCGProcessor.csproj -c Release -o /app --no-restore

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the built app
COPY --from=build /app ./

# Create a non-root user and group
RUN addgroup --system --gid 1000 appgroup && \
    adduser --system --uid 1000 --gid 1000 --disabled-password --shell /bin/sh appuser && \
    chown -R appuser:appgroup /app

USER appuser

# Expose the port your app runs on
EXPOSE 80

# Set the entry point (environment variables will be automatically picked up)
ENTRYPOINT ["dotnet", "TCGProcessor.dll"]