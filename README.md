# HelpDesk API - Sistema de Chamados em Cloud

Sistema profissional de gerenciamento de chamados (Help Desk) construido com **Clean Architecture**, **ASP.NET Core 8**, **PostgreSQL** e deploy na **AWS**.

---

## Tecnologias

| Camada | Tecnologia |
|--------|-----------|
| Backend | ASP.NET Core 8 Web API |
| Banco de Dados | PostgreSQL 16 |
| ORM | Entity Framework Core |
| Autenticacao | JWT Bearer Tokens |
| Documentacao | Swagger / OpenAPI |
| Logs | Serilog (Console + File) |
| Mapeamento | AutoMapper |
| Testes | xUnit + Moq + FluentAssertions |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Cloud | AWS (ECR + ECS + RDS) |

---

## Arquitetura

O projeto segue **Clean Architecture** com separacao clara de responsabilidades:

```
src/
  HelpDesk.Domain/          -> Entidades, Enums, Interfaces
  HelpDesk.Application/     -> DTOs, Services, Mappings
  HelpDesk.Infrastructure/  -> DbContext, Repositories, TokenService
  HelpDesk.API/             -> Controllers, Middleware, Program.cs
tests/
  HelpDesk.Tests/           -> Testes unitarios
```

---

## Funcionalidades

### Autenticacao & Autorizacao
- Registro e login com JWT
- 3 roles: **Admin**, **Tecnico**, **Usuario**
- Endpoints protegidos por role

### Chamados (Tickets)
- Criar, editar, listar com **paginacao e filtros**
- Atribuir tecnico responsavel
- Alterar status: Aberto -> Em Andamento -> Resolvido -> Fechado
- Upload de anexos
- **Audit log** completo de todas as alteracoes

### Comentarios
- Adicionar comentarios em chamados
- Historico completo de interacoes

### Dashboard (Metricas)
- Total de chamados por status
- Tempo medio de resolucao
- Chamados por tecnico
- Chamados por categoria

### Administracao
- Gerenciar usuarios e roles
- Gerenciar categorias

---

## Endpoints da API

| Metodo | Rota | Descricao | Role |
|--------|------|-----------|------|
| POST | `/api/auth/register` | Registrar usuario | Publico |
| POST | `/api/auth/login` | Login | Publico |
| GET | `/api/tickets` | Listar chamados (paginado) | Autenticado |
| GET | `/api/tickets/{id}` | Detalhes do chamado | Autenticado |
| GET | `/api/tickets/my` | Meus chamados | Autenticado |
| POST | `/api/tickets` | Criar chamado | Autenticado |
| PUT | `/api/tickets/{id}` | Editar chamado | Autenticado |
| PUT | `/api/tickets/{id}/assign` | Atribuir tecnico | Admin/Tecnico |
| PUT | `/api/tickets/{id}/status` | Alterar status | Admin/Tecnico |
| GET | `/api/tickets/{id}/comments` | Listar comentarios | Autenticado |
| POST | `/api/tickets/{id}/comments` | Adicionar comentario | Autenticado |
| GET | `/api/categories` | Listar categorias | Autenticado |
| POST | `/api/categories` | Criar categoria | Admin |
| GET | `/api/users` | Listar usuarios | Admin |
| PUT | `/api/users/{id}/role` | Alterar role | Admin |
| GET | `/api/dashboard/stats` | Metricas | Admin/Tecnico |

---

## Como Rodar Localmente

### Pre-requisitos
- .NET 8 SDK
- Docker e Docker Compose
- PostgreSQL (ou use Docker)

### Opcao 1: Docker Compose (Recomendado)

```bash
docker-compose up --build
```

A API estara disponivel em: `http://localhost:5000`
Swagger: `http://localhost:5000/swagger`

### Opcao 2: Rodar manualmente

```bash
# 1. Subir PostgreSQL via Docker
docker run -d --name helpdesk-db \
  -e POSTGRES_DB=helpdesk \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine

# 2. Rodar a API
cd src/HelpDesk.API
dotnet run
```

### Rodar Testes

```bash
dotnet test
```

---

## Credenciais Padrao

| Email | Senha | Role |
|-------|-------|------|
| admin@helpdesk.com | Admin@123 | Admin |

---

## Deploy na AWS

### Infraestrutura necessaria:
1. **ECR** - Container registry para a imagem Docker
2. **ECS** (Fargate) - Execucao dos containers
3. **RDS** (PostgreSQL) - Banco de dados gerenciado
4. **S3** - Armazenamento de anexos (opcional)

### Variaveis de ambiente (Secrets do GitHub):
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`

### Pipeline CI/CD:
1. Push na branch `main`
2. GitHub Actions builda e testa
3. Constroi imagem Docker
4. Push para ECR
5. Deploy automatico no ECS

---

## Estrutura do Banco de Dados

```
Users          -> Usuarios do sistema
Tickets        -> Chamados
Comments       -> Comentarios nos chamados
Categories     -> Categorias dos chamados
AuditLogs      -> Log de auditoria
```

---

## Diferenciais Tecnicos

- **Clean Architecture** com separacao clara de camadas
- **JWT Authentication** com roles
- **Swagger** documentado com autorizacao
- **Serilog** para logs estruturados
- **Docker** multi-stage build
- **CI/CD** com GitHub Actions
- **Paginacao** e **filtros** nos endpoints
- **Audit Log** automatico
- **Seed data** com usuario admin e categorias
- **Exception Middleware** global
- **Testes unitarios** com xUnit + Moq
