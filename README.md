# Painel de Projetos — PVT

Módulo do Portal de Gestão que mostra, para cada projeto, as horas **vendidas**, **planejadas** e **realizadas**, o **saldo** de horas e um **status** (Saudável, Atenção ou Crítico).

📄 **O desenho completo da solução (Parte 1) está em [`docs/DESIGN.md`](docs/DESIGN.md).**

---

## Como rodar

**Pré-requisitos:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) e [Docker](https://www.docker.com/).

```bash
# 1. Subir o SQL Server
docker compose up -d

# 2. Rodar a API (aguarde uns 20s após o passo 1, enquanto o SQL Server inicia)
cd backend
dotnet run --project src/PainelProjetos.Api
```

Ao iniciar em ambiente de desenvolvimento, a API **cria o banco e insere os dados de exemplo** automaticamente.

- Painel: http://localhost:5190/api/projetos
- Swagger: http://localhost:5190/swagger

**Testes:**

```bash
cd backend
dotnet test
```

> O SQL Server roda na porta **14330**, e não na padrão 1433, para não conflitar com um SQL Server que já esteja instalado na máquina.

---

## O endpoint

`GET /api/projetos` — filtros opcionais: `status` (`Saudavel`, `Atencao`, `Critico`), `clienteId` e `situacao` (padrão: `Ativo`).

```json
{
  "sincronizadoEm": "2026-10-12T23:55:13Z",
  "projetos": [
    {
      "id": 4,
      "nome": "Implantação RM Folha",
      "cliente": "Metalúrgica Serra",
      "horasVendidas": 200.00,
      "horasPlanejadas": 180.00,
      "horasRealizadas": 170.00,
      "saldo": 30.00,
      "consumo": 85.00,
      "avancoFisico": 60.00,
      "status": "Critico"
    }
  ]
}
```

### Dados de exemplo

Representam o que já teria sido sincronizado do TOTVS RM e registrado no portal. Cada projeto cobre uma regra de status (seção 4.4 do `DESIGN.md`):

| Projeto | Consumo | Avanço físico | Status |
|---|---|---|---|
| Portal do Colaborador | 30% | 30% | Saudável |
| Migração Contábil | 82,5% | 80% | Atenção (consumo ≥ 80%) |
| Implantação RM Saúde | 105% | 90% | Crítico (consumo ≥ 100%) |
| Implantação RM Folha | 85% | 60% | Crítico (desvio de 25 pontos) |

O Portal do Colaborador tem também um apontamento **pendente** de 8h, que não entra no realizado, porque só contam as horas já confirmadas pelo RM.

---

## Principais decisões

- **Duas medidas de avanço:** *consumo* (horas gastas ÷ vendidas) e *avanço físico* (peso das etapas concluídas). Comparar as duas revela a entrega atrasada em relação ao financeiro.
- **Status pela regra mais grave:** Crítico se consumo ≥ 100% ou desvio ≥ 20 pontos; Atenção se consumo ≥ 80% ou desvio ≥ 10 pontos. Os limites ficam no `appsettings.json`.
- **Indicadores calculados, não armazenados:** sempre consistentes com os dados de origem. As somas são feitas pelo SQL Server.
- **Regra isolada do banco:** a `CalculadoraIndicadores` recebe números e devolve o status, o que permite testá-la sem banco.
- **O painel lê do banco local, nunca do RM:** continua rápido e disponível mesmo com o ERP lento ou fora do ar.
- **Apontamentos via padrão Outbox:** gravados como pendentes e enviados ao RM em segundo plano.

Detalhes, alternativas descartadas e premissas estão no [`DESIGN.md`](docs/DESIGN.md).

---

## Escopo do código

**Implementado:** modelo de dados completo (6 entidades), banco SQL Server via Docker, `GET /api/projetos` com filtros, regra de saldo e status, e testes automatizados da regra.

**Descrito no `DESIGN.md`, mas não implementado:** os workers de sincronização e Outbox, a integração com o RM (`IRmClient`) e os demais endpoints. Os dados de exemplo fazem o papel do que a sincronização traria.

---

## Estrutura

```
├── docs/DESIGN.md            Documento de design (Parte 1)
├── docker-compose.yml        SQL Server
└── backend/
    ├── src/PainelProjetos.Api/
    │   ├── Controllers/      Entrada HTTP
    │   ├── Services/         Regras de negócio
    │   ├── Data/             DbContext e dados de exemplo
    │   ├── Models/           Entidades
    │   ├── Dtos/             Formato das respostas
    │   └── Migrations/       Criação do banco
    └── tests/PainelProjetos.Tests/
```

**Stack:** .NET 8 · ASP.NET Core Web API · Entity Framework Core · SQL Server · xUnit · Docker
