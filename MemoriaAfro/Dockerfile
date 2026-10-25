# ==== AJUSTE AQUI ====
# WEB_PROJECT = nome da pasta/projeto que contém o Program.cs (o .csproj deve ter o mesmo nome)
# DOTNET_VERSION = versão do .NET do seu projeto (ver <TargetFramework> no .csproj: net8.0 -> 8.0)
ARG WEB_PROJECT=MemoriaAfro.Web
ARG DOTNET_VERSION=8.0

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG WEB_PROJECT
WORKDIR /src

# Copia a solução inteira (Core, Infra, Web...) para o build enxergar todos os projetos
COPY . .

RUN dotnet restore "${WEB_PROJECT}/${WEB_PROJECT}.csproj"
RUN dotnet publish "${WEB_PROJECT}/${WEB_PROJECT}.csproj" -c Release -o /app --no-restore

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
ARG WEB_PROJECT
WORKDIR /app
COPY --from=build /app .

ENV APP_DLL=${WEB_PROJECT}.dll
ENV ASPNETCORE_ENVIRONMENT=Production

# A porta vem da variável PORT do Render e é lida no Program.cs
ENTRYPOINT ["sh", "-c", "dotnet $APP_DLL"]