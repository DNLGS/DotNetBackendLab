# DotNetFoundations

Projeto de estudos para aprofundar C#, .NET e desenvolvimento de APIs, com foco em fundamentos de backend e preparação para entrevistas técnicas.

## Tecnologias

- .NET 10 e ASP.NET Core Web API.
- Entity Framework Core e PostgreSQL, com Npgsql.
- OpenAPI.

## O que estou aprofundando

- EF Core: `DbContext`, `DbSet`, persistência, relacionamentos e migrations.
- Injeção de dependência e ciclo de vida dos serviços.
- Generics, interfaces e herança.
- Organização em controllers, services e repositories.
- Programação assíncrona com `Task`, `async/await` e cancelamento.
- Middlewares, tratamento global de exceções e respostas com `ProblemDetails`.
- Logs estruturados e configuração por ambiente.
- Modelagem de clientes, produtos, pedidos e itens.

## Próximos estudos

- Consultas com LINQ, filtros, paginação e CRUD completo.
- Transações, isolamento, concorrência e idempotência.
- Testes unitários e de integração.
- Consumo de APIs com `IHttpClientFactory`.
- Autenticação, autorização e semântica HTTP.

## Configuração local

O projeto utiliza .NET 10 e PostgreSQL. Configure a conexão em `ConnectionStrings:DefaultConnection` no seu `appsettings.json` local.

Os arquivos `appsettings.json` e `appsettings.Development.json` são ignorados pelo Git para manter as configurações e credenciais locais fora do repositório.

Projeto em evolução: os tópicos acima representam estudos em andamento.

## DTOs e contratos

Os DTOs estão separados por entidade e operação. Consulte [os contratos e exemplos](docs/DTOs.md) para criação, filtros, respostas, atualização e exclusão. O valor bruto dos pedidos é calculado no serviço a partir dos itens. PUT e DELETE exigem a versão recebida no GET; conflitos retornam 409. O filtro de data usa dias UTC.

As migrações de valores para decimal e de proteção dos relacionamentos estão preparadas. A nova API exige aplicar `20261022120000_ProtectOrderIntegrity`, incluindo as migrações anteriores pendentes, antes do uso. Clientes/produtos vinculados têm exclusão bloqueada; excluir um pedido remove seus itens. Não houve aplicação de migrações nem validação em execução nesta alteração. Autenticação permanece como próxima etapa.
