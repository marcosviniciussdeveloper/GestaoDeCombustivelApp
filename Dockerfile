# Etapa base: runtime do ASP.NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# Etapa build: SDK do .NET 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos da solução
COPY ./WebApplication1 ./WebApplication1

# Restaura e publica
RUN dotnet restore "WebApplication1/WebApplication1.csproj"
RUN dotnet publish "WebApplication1/WebApplication1.csproj" -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
