FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Kopiowanie plików projektu
COPY ["Tabula.Services.WebApi/Tabula.Services.WebApi.csproj", "./"]
RUN dotnet restore "Tabula.Services.WebApi.csproj"

# Kopiowanie całego kodu źródłowego
COPY Tabula.Services.WebApi/. .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080 8443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tabula.Services.WebApi.dll"]

