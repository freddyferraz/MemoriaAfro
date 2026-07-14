FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia somente os .csproj primeiro, para aproveitar cache de camadas do Docker
COPY MemoriaAfro/MemoriaAfro.csproj MemoriaAfro/
COPY MemoriaAfro.Core/MemoriaAfro.Core.csproj MemoriaAfro.Core/
COPY MemoriaAfro.Infra/MemoriaAfro.Infra.csproj MemoriaAfro.Infra/
RUN dotnet restore MemoriaAfro/MemoriaAfro.csproj

# Copia o restante do código e publica
COPY . .
RUN dotnet publish MemoriaAfro/MemoriaAfro.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MemoriaAfro.dll"]
