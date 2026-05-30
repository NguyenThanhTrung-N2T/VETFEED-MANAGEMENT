# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["VETFEED.Backend.API/VETFEED.Backend.API.csproj", "VETFEED.Backend.API/"]
RUN dotnet restore "VETFEED.Backend.API/VETFEED.Backend.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/VETFEED.Backend.API"
RUN dotnet build "VETFEED.Backend.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "VETFEED.Backend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VETFEED.Backend.API.dll"]
