# INDT - PropostaService

Este é um projeto de serviço de propostas desenvolvido em .NET 8.0.

## Descrição

O PropostaService é uma API web desenvolvida em ASP.NET Core que fornece funcionalidades para gerenciamento de propostas.

## Tecnologias Utilizadas

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI** para documentação da API
- **Docker** para containerização

## Estrutura do Projeto

```
INDT/
├── INDT.sln                    # Arquivo de solução do Visual Studio
├── PropostaService/            # Projeto principal da API
│   ├── Controllers/           # Controladores da API
│   ├── Properties/            # Configurações de propriedades
│   ├── appsettings.json       # Configurações da aplicação
│   ├── Program.cs             # Ponto de entrada da aplicação
│   └── PropostaService.csproj # Arquivo de projeto
└── README.md                  # Este arquivo
```

## Como Executar

### Pré-requisitos

- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code

### Executando Localmente

1. Clone o repositório
2. Navegue até a pasta do projeto:
   ```bash
   cd PropostaService
   ```
3. Restaure as dependências:
   ```bash
   dotnet restore
   ```
4. Execute o projeto:
   ```bash
   dotnet run
   ```
5. Acesse a API em: `https://localhost:7001` ou `http://localhost:5000`
6. Acesse a documentação Swagger em: `https://localhost:7001/swagger`

### Executando com Docker

1. Construa a imagem Docker:
   ```bash
   docker build -t proposta-service .
   ```
2. Execute o container:
   ```bash
   docker run -p 8080:80 proposta-service
   ```

## Endpoints Disponíveis

- `GET /weatherforecast` - Endpoint de exemplo do WeatherForecast

## Desenvolvimento

Este projeto está configurado para desenvolvimento com:
- Hot reload habilitado
- Configurações de desenvolvimento separadas
- Suporte a Docker para desenvolvimento

## Licença

Este projeto é parte do sistema INDT. 