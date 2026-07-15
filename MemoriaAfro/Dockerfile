# Estágio de Compilação (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto de todas as camadas a partir da raiz da solução
COPY ["MemoriaAfro/MemoriaAfro.csproj", "MemoriaAfro/"]
COPY ["MemoriaAfro.Core/MemoriaAfro.Core.csproj", "MemoriaAfro.Core/"]
COPY ["MemoriaAfro.Infra/MemoriaAfro.Infra.csproj", "MemoriaAfro.Infra/"]

# Restaura as dependências apontando para o projeto principal
RUN dotnet restore "MemoriaAfro/MemoriaAfro.csproj"

# Copia todo o restante do código da solução
COPY . .

# Entra na pasta do projeto principal e compila
WORKDIR "/src/MemoriaAfro"
RUN dotnet publish -c Release -o /app/publish

# Estágio de Execução (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MemoriaAfro.dll"]