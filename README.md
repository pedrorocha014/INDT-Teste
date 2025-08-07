# INDT - Sistema de Contratação de Seguros

Este projeto implementa um sistema de contratação de seguros baseado em microserviços, utilizando arquitetura limpa e comunicação assíncrona via Kafka.

## Notas

Algumas observações:

1 - Estou utilizando versões bastante simplificadas das entidades de contrato e proposta, pois o foco principal foi explorar a arquitetura do projeto com microserviços.

2 - Pelo mesmo motivo do item anterior, não implementei validações de domínio, como a verificação do CPF, por exemplo.

3 - O teste ContratacaoIntegrationTests está funcional, mas aparentemente não finaliza corretamente após o Assert. Não consegui investigar esse comportamento a tempo.


## Arquitetura

O projeto é composto pelos seguintes componentes:

- **PropostaService**: API para gerenciamento de propostas
- **ContratacaoService**: API para contratação de propostas aprovadas
- **ContratacaoWorker**: Worker que processa eventos de contratação via Kafka
- **Infrastructure**: Camada de infraestrutura com Entity Framework e configurações
- **Applications**: Camada de aplicação com usecases e handlers
- **Core**: Camada de domínio com entidades e agregados

## Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- Docker Compose
- PostgreSQL (compose)
- Kafka (compose)

### 1. Iniciar Infraestrutura

Primeiro, inicie os serviços essenciais (PostgreSQL, Kafka, Zookeeper):

```bash
docker compose -f SolutionItems/compose.yaml --profile essentials up -d --build
```

### 2. Executar Migrações

Aplique as migrações do banco de dados:

```bash
dotnet ef database update --project src/Infrastructure -- --connection "Server=localhost;Port=5433;Database=indt_db;Username=postgres;Password=postgres"
```

### 3. Executar Aplicações

Execute as três aplicações em paralelo, via Cli, Bash, Visual Std, como quiser:

```bash
dotnet run --project src/PropostaService/PropostaService.csproj & dotnet run --project src/ContratacaoService/ContratacaoService.csproj & dotnet run --project src/ContratacaoWorker/ContratacaoWorker.csproj
```

**Portas das aplicações com swagger:**
- PropostaService: http://localhost:5276/swagger/index.html
- ContratacaoService: http://localhost:5203/swagger/index.html
- ContratacaoWorker: Executa em background


## Fluxo Principal da Aplicação

### 1. Criar uma Proposta

```bash
curl -X POST http://localhost:5276/propostas \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "cpf": "12345678901"
  }'
```

**Resposta esperada:**
```json
{
  "id": "guid-da-proposta",
  "name": "João Silva",
  "cpf": "12345678901",
  "status": "EmAnalise",
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-01T10:00:00Z"
}
```

### 2. Alterar Status da Proposta para Aprovada

```bash
curl -X PUT http://localhost:5276/propostas/{id} \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Aprovada"
  }'
```

**Status possíveis:** `EmAnalise`, `Aprovada`, `Rejeitada`

### 3. Contratar a Proposta Aprovada

```bash
curl -X POST http://localhost:5203/contratacoes \
  -H "Content-Type: application/json" \
  -d '{
    "propostaId": id-da-proposta-aprovada
  }'
```

### 4. Verificar Contrato Criado

```bash
curl -X GET http://localhost:5203/contratacoes
```

**Resposta esperada:**
```json
[
  {
    "id": "guid-do-contrato",
    "propostaId": "id-da-proposta",
    "createdAt": "2024-01-01T10:00:00Z",
    "propostaSeguro": {
      "id": "id-da-proposta",
      "name": "João Silva",
      "cpf": "12345678901",
      "status": "Aprovada",
      "createdAt": "2024-01-01T10:00:00Z",
      "updatedAt": "2024-01-01T10:00:00Z"
    }
  }
]
```

## Processamento Assíncrono

O **ContratacaoWorker** processa automaticamente as contratações:

1. Quando uma proposta é contratada via API, um evento é publicado no Kafka
2. O worker consome esse evento e cria o contrato no banco de dados

## Testes

### Testes de Integração

Para executar os testes de integração:

```bash
# Iniciar banco de teste
docker compose -f SolutionItems/compose.yaml --profile testing up -d --build

dotnet ef database update --project src/Infrastructure -- --connection "Server=localhost;Port=5434;Database=indt_test_db;Username=postgres;Password=postgres"

# Executar testes
dotnet test tests/PropostaIntegrationTests/PropostaIntegrationTests.csproj
dotnet test tests/ContratacaoIntegrationTests/ContratacaoIntegrationTests.csproj
```