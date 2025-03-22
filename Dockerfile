FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY . ./
# Definir argumento para a senha do NuGet
ARG ARG_SECRET_NUGET_PACKAGES
# Adicionar a fonte privada do GitHub Packages
RUN dotnet nuget remove source github || echo "Fonte não encontrada, continuando..."
RUN dotnet nuget add source "https://nuget.pkg.github.com/caiofabiogomes/index.json" 

RUN dotnet restore

RUN dotnet publish TCFiapConsumerDeleteContact.API/TCFiapConsumerDeleteContact.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

COPY --from=build-env /App/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "TCFiapConsumerDeleteContact.API.dll"]
