FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Definir argumento para a senha do NuGet
ARG ARG_SECRET_NUGET_PACKAGES

COPY . ./

# Criar um arquivo nuget.config com clear text credentials para ambos os feeds
RUN echo "<?xml version=\"1.0\" encoding=\"utf-8\"?>
<configuration>
  <packageSources>
    <add key=\"github\" value=\"https://nuget.pkg.github.com/caiofabiogomes/index.json\" />
    <add key=\"github-techchallengefiap\" value=\"https://nuget.pkg.github.com/caiofabiogomes/download/techchallengefiap/index.json\" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key=\"Username\" value=\"ErickGoldberg\" />
      <add key=\"ClearTextPassword\" value=\"$ARG_SECRET_NUGET_PACKAGES\" />
    </github>
    <github-techchallengefiap>
      <add key=\"Username\" value=\"ErickGoldberg\" />
      <add key=\"ClearTextPassword\" value=\"$ARG_SECRET_NUGET_PACKAGES\" />
    </github-techchallengefiap>
  </packageSourceCredentials>
</configuration>" > nuget.config

# Usar o nuget.config para restaurar os pacotes
RUN dotnet restore --configfile nuget.config

RUN dotnet publish TCFiapConsumerDeleteContact.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

COPY --from=build-env /App/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "TCFiapConsumerDeleteContact.API.dll"]
