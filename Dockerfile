# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo da raiz do projeto para dentro do container
COPY . .

# Restaura as dependências da solução
RUN dotnet restore WebApplication1/WebApplication1.sln

# Publica o projeto principal
RUN dotnet publish WebApplication1/WebApplication1.csproj -c Release -o /app/publish

# Etapa 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define variáveis e porta
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Executa a aplicação
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
