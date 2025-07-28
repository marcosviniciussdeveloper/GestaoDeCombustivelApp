# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj e restaura as dependências
COPY WebApplication1/*.csproj ./WebApplication1/
RUN dotnet restore WebApplication1/WebApplication1.csproj

# Copia todo o código e publica o build
COPY . .
WORKDIR /app/WebApplication1
RUN dotnet publish -c Release -o /out

# Etapa 2: imagem final com runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expõe a porta da aplicação
EXPOSE 80

ENTRYPOINT ["dotnet", "WebApplication1.dll"]
