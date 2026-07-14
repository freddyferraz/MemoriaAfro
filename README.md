# Memória Afro — Solução em Clean Architecture

Reorganização do projeto em **3 camadas**, seguindo a estrutura que você já
tinha no Visual Studio:

```
MemoriaAfro.sln
├── MemoriaAfro/            (Web API — Controllers, DTOs, Program.cs)
├── MemoriaAfro.Core/       (Domínio — Models, Enum, Interfaces, Services*)
└── MemoriaAfro.Infra/      (Infraestrutura — Data, Migrations, Repositories, Services*)
```
`*` Core define as **interfaces** dos serviços (`ITokenService`, `IPontosService`);
o Infra contém as **implementações** concretas. Isso segue a Regra de
Dependência: Core não referencia nada, Infra referencia Core, Web referencia os dois.

## ⚠️ Decisão que tomei sozinho — confirme se faz sentido

Você tinha `HistoricoPontos` + `TipoMovimentacao` + `UploadMemoria` + `Usuario`
como stubs vazios. Interpretei isso como um **programa de pontos de fidelidade**:
o usuário ganha pontos (`TipoMovimentacao.Credito`) quando um `UploadMemoria`
dele é aprovado, pensando no resgate futuro no e-commerce (`TipoMovimentacao.Debito`).
Isso ficou centralizado em `IPontosService` (Core) / `PontosService` (Infra),
chamado automaticamente pelo `UploadsController` quando um upload é aprovado.

**Se a intenção de `HistoricoPontos` era outra coisa (ex.: histórico de locais
visitados no mapa), me avise que eu reformulo.**

## Por que separei `Usuario` (Core) de `ApplicationIdentityUser` (Infra)?

O ASP.NET Core Identity é um detalhe de infraestrutura (framework de auth).
Para o `Core` não depender dele:
- `MemoriaAfro.Infra.Data.ApplicationIdentityUser` guarda credenciais (Identity puro).
- `MemoriaAfro.Core.Models.Usuario` guarda dados de domínio (nome, uploads, pontos).
- Os dois compartilham o mesmo `Id` (gerado pelo Identity no registro).

Isso dá mais trabalho no `AuthController` (precisa criar os dois registros),
mas mantém o `Core` limpo — importante se um dia vocês quiserem reusar as
regras de negócio fora de uma API ASP.NET Core (ex.: um worker/job).

## Como rodar localmente

```bash
dotnet restore
```

Configure a connection string em `MemoriaAfro/appsettings.json` (Postgres) e
troque `Jwt:Key`. Depois:

```bash
dotnet tool install --global dotnet-ef   # se ainda não tiver
dotnet ef migrations add InitialCreate --project MemoriaAfro.Infra --startup-project MemoriaAfro
dotnet ef database update --project MemoriaAfro.Infra --startup-project MemoriaAfro
dotnet run --project MemoriaAfro
```

> Note o `--project`/`--startup-project`: como o `DbContext` mora no Infra mas
> quem tem o `Program.cs` (e a connection string) é o Web, o `dotnet ef` precisa
> saber onde está cada coisa.

## Docker

O `Dockerfile` está dentro de `MemoriaAfro/`, mas o *build context* precisa ser
a raiz da solução (para copiar os 3 `.csproj`):

```bash
docker build -f MemoriaAfro/Dockerfile -t memoria-afro-api .
```

## Mapeamento de nomes (versão anterior → esta versão)

| Antes | Agora | Motivo |
|---|---|---|
| `ContribuicaoAcervo` | `UploadMemoria` | Nome já existente no seu scaffold |
| `ApplicationUser` | `Usuario` (Core) + `ApplicationIdentityUser` (Infra) | Separação de domínio vs. auth |
| `PontoHistorico` | `PontoInteresse` | Evitar colisão com `HistoricoPontos` (fidelidade) |
| `StatusContribuicao` | `StatusUpload` | Nome já existente no seu scaffold |
| — | `HistoricoPontos` / `TipoMovimentacao` | Novo: ledger de pontos de fidelidade |

## Endpoints (resumo)

- `POST /api/auth/registrar`, `POST /api/auth/login`
- `GET/POST/PATCH /api/uploads`, `GET /api/uploads/{id}/imagem`
- `GET /api/historicopontos/saldo`, `GET /api/historicopontos/extrato`
- `GET/POST/PUT/DELETE /api/pontosinteresse`
- `GET/POST/PUT/DELETE /api/documentarios`
- `GET/POST/DELETE /api/arquivosimersivos`
- `GET/POST/DELETE /api/tutoriais`

## Próximos passos sugeridos

- Endpoint para promover usuário a `Admin`/`Moderador`.
- Paginação nas listagens.
- Testes com NSubstitute para os repositórios/serviços (mockando as interfaces do Core).
- Rate limiting em `POST /api/uploads` (endpoint público).
