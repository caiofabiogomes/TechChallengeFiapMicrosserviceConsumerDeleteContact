# Stage: base – imagem de runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Stage: build – utiliza a imagem do SDK para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o NuGet.config (que referencia o /src/LocalPackages)
COPY NuGet.config .

# Copia a pasta LocalPackages para que o restore encontre os pacotes locais
COPY LocalPackages ./LocalPackages

# Copia o arquivo de projeto da API
COPY ["TCFiapConsumerDeleteContact.API.csproj", "./"]
RUN dotnet restore "TCFiapConsumerDeleteContact.API.csproj"

# Copia todo o restante do código para o container
COPY . .

# Define o diretório de trabalho para o projeto da API e compila
WORKDIR /src
RUN dotnet build "TCFiapConsumerDeleteContact.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage: publish – publica a aplicação para produção
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TCFiapConsumerDeleteContact.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage final – constrói a imagem de runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TCFiapConsumerDeleteContact.API.dll"]
