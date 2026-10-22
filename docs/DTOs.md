# Contratos da API

Os controllers recebem DTOs de `DTOs/<entidade>/Requests` e os serviços retornam os de `Responses`. As entidades ficam em `Models/Entities`, mantendo o namespace `DotNetFoundations.Models` para preservar sua identidade no EF. As conversões são explícitas nos serviços; o repositório continua recebendo entidades.

## Operações disponíveis e preparadas

| Entidade | Criação | Consulta | Atualização preparada | Exclusão preparada | Resposta |
| --- | --- | --- | --- | --- | --- |
| Clientes | CreateClientRequest | QueryClientRequest | UpdateClientRequest | DeleteClientRequest | ClientResponse |
| Produtos | CreateProductRequest | QueryProductRequest | UpdateProductRequest | DeleteProductRequest | ProductResponse |
| Pedidos | CreateOrderRequest | QueryOrderRequest | UpdateOrderRequest | DeleteOrderRequest | OrderResponse |
| Itens | CreateItemRequest | QueryItemRequest | UpdateItemRequest | DeleteItemRequest | ItemResponse |

Estão disponíveis POST, GET, PUT e DELETE em `/api/Clients`, `/api/Products` e `/api/Orders`. Itens são enviados dentro de pedidos; não existe controller de itens nem operação HTTP independente para eles.

Produtos e pedidos exigem todos os campos obrigatórios do DTO de atualização, incluindo seu identificador. Clientes preservam os valores atuais dos campos omitidos ou nulos; nome informado em branco é inválido. Em UpdateOrderRequest, a lista de itens é obrigatória e substitui a composição atual: cada UpdateItemRequest informa um itemsId existente no mesmo pedido, sem duplicados; itens omitidos são excluídos. IDs e datas de criação dos registros mantidos são preservados. Para incluir novos itens, o contrato de atualização precisará ser ampliado.

PUT e DELETE de clientes, produtos e pedidos exigem `version`, um inteiro sem sinal maior que zero obtido no GET. PUT recebe o campo no JSON; DELETE recebe o identificador e `version` na query string. Versão ausente ou zero gera 400; versão desatualizada gera 409. Clientes da API que enviavam apenas o identificador precisam incluir esse campo. Depois de atualizar, consulte novamente para obter a nova versão.

`Version` é mapeada à coluna de sistema `xmin` do PostgreSQL, usando `uint` e `IsRowVersion()`, sem criar uma coluna física nova. A comparação é feita antes de alterar a entidade e também no UPDATE/DELETE do EF Core. Uma alteração apenas nos itens força a atualização do cabeçalho para renovar a versão do pedido. O repositório preserva o token e salva somente as alterações das entidades já rastreadas. Referência do provedor: [concorrência com xmin](https://www.npgsql.org/efcore/modeling/concurrency.html).

Exemplos (substitua o identificador e a versão pelos retornados no GET):

```http
PUT /api/Clients
Content-Type: application/json

{"clientsId":"11111111-1111-1111-1111-111111111111","version":12345,"name":"Nome atualizado"}
```

```http
DELETE /api/Orders?OrdersId=11111111-1111-1111-1111-111111111111&Version=12345
```

Clientes com pedidos e produtos utilizados em itens não podem ser excluídos: as restrições do banco geram 409, sem remover o histórico. Excluir um pedido remove seus itens na mesma operação por cascata no banco. Essas regras dependem da aplicação da migração `20261022120000_ProtectOrderIntegrity` antes de usar a nova versão da API. Conflitos de chave estrangeira, inclusive uma referência removida entre a validação e a gravação, retornam 409 sem expor detalhes internos do banco.

CreateObjectByDTO é síncrono nos quatro services: valida as anotações dos DTOs e converte criação e atualização para entidades. Itens aninhados são validados individualmente, incluindo overflow do produto quantidade × preço. Criação e atualização de pedidos consultam cliente e produtos com await nos métodos assíncronos antes da persistência; ambas recalculam e validam os totais. DTO nulo ou não suportado é rejeitado.

## Campos de criação

| DTO | Campos obrigatórios | Campos opcionais / padrões |
| --- | --- | --- |
| CreateClientRequest | name: string não vazia | email: string/null com formato de e-mail; phone: string/null com formato de telefone |
| CreateProductRequest | name: string não vazia | suggestedPrice: decimal >= 0, padrão 0 |
| CreateOrderRequest | clientId: Guid não vazio; items: lista de CreateItemRequest | status: OrderStatus, padrão 0; descount e increase: decimal >= 0, padrão 0 |
| CreateItemRequest | productId: Guid não vazio; quantity: decimal > 0; unitPrice: decimal >= 0 | Nenhum |

A lista de itens deve estar presente e não pode conter elementos nulos. Uma lista vazia continua permitida, preservando o comportamento anterior. GUIDs de consulta são opcionais, mas não podem ser Guid.Empty quando informados. Nomes em branco, enums desconhecidos e valores negativos são inválidos. A API usa a validação automática de `[ApiController]` e retorna 400 para contratos inválidos.

IDs gerados, createdAt, grossValue, total e objetos de navegação não fazem parte da criação. Campos JSON desconhecidos seguem o comportamento padrão do serializador: são ignorados e não alteram a entidade. Cliente e produtos precisam existir antes da gravação de um pedido; referência inexistente gera 400 pelo middleware atual.

OrderStatus mantém valores numéricos: Pending = 0, Processing = 1, Shipped = 2, Delivered = 3, Cancelled = 4. Exemplos JSON para o campo: `{"status":0}`, `{"status":1}`, `{"status":2}`, `{"status":3}`, `{"status":4}`. O nome `descount` foi preservado.

## Exemplos de criação

POST /api/Clients:

```json
{"name":"Cliente exemplo","email":"cliente@example.com","phone":"11999999999"}
```

POST /api/Products:

```json
{"name":"Produto exemplo","suggestedPrice":12.50}
```

POST /api/Orders (substitua os GUIDs por registros existentes):

```json
{
  "clientId":"11111111-1111-1111-1111-111111111111",
  "status":0,
  "descount":2.50,
  "increase":1.00,
  "items":[
    {"productId":"22222222-2222-2222-2222-222222222222","quantity":2,"unitPrice":10.50},
    {"productId":"33333333-3333-3333-3333-333333333333","quantity":1.5,"unitPrice":4.00}
  ]
}
```

O serviço calcula grossValue = 2 × 10.50 + 1.5 × 4.00 = 27.00 e total = 27.00 - 2.50 + 1.00 = 25.50. O preço é informado por item, sem buscar SuggestedPrice para substituí-lo. Não há regra nova de arredondamento nem de limite do desconto pelo valor bruto. Overflow decimal gera 400 antes da persistência.

As criações continuam retornando HTTP 200 e somente a mensagem existente:

```json
{"message":"Client created successfully"}
```

```json
{"message":"Product added successfully"}
```

```json
{"message":"Order created successfully"}
```

## Consultas e respostas

Os três DTOs de consulta usados pelos GETs são aplicados diretamente pelos services, como QueryRequest, e as entidades resultantes são convertidas em DTOs de resposta. Todos incluem page (padrão 1), pageSize (padrão 20), orderBy (nome de propriedade persistida, exceto Version) e descending (padrão false). Mantém-se a normalização de page para no mínimo 1 e pageSize para o intervalo 1–100. O deslocamento é calculado como long; valores acima de int.MaxValue são rejeitados com 400 antes do Skip, sem overflow.

| Consulta | Filtros específicos opcionais |
| --- | --- |
| QueryClientRequest | name, nameContains, email, emailContains, phone: string |
| QueryProductRequest | name, nameContains: string; minSuggestedPrice: decimal >= 0 |
| QueryOrderRequest | clientId: Guid; status: OrderStatus; descount, increase, grossValue: decimal >= 0; createdAt: DateTime |
| QueryItemRequest (sem rota) | ordersId, productId: Guid |

Exemplos: `GET /api/Products?NameContains=exemplo&Page=1&PageSize=20` e `GET /api/Orders?Status=0&OrderBy=CreatedAt&Descending=true`. CreatedAt filtra um dia UTC: `GET /api/Orders?CreatedAt=2026-10-22` usa o intervalo [2026-10-22T00:00:00Z, 2026-10-23T00:00:00Z). Datas sem fuso representam o dia UTC informado; valores com fuso seguem a conversão UTC do model binding do ASP.NET Core. Os limites enviados ao Npgsql são DateTime UTC. A data máxima, sem dia seguinte representável, é rejeitada com 400.

GET preserva os envelopes `{ "clients": [] }`, `{ "products": [] }` e `{ "orders": [] }`.

| Resposta | Campos |
| --- | --- |
| ClientResponse | clientsId: Guid; version: uint; name: string; email e phone: string/null; createdAt: DateTime |
| ProductResponse | productsId: Guid; version: uint; name: string; suggestedPrice: decimal; createdAt: DateTime |
| ItemResponse | itemsId: Guid; ordersId: Guid/null; productId: Guid; product: ProductResponse/null; quantity, unitPrice e total: decimal; createdAt: DateTime |
| OrderResponse | ordersId e clientId: Guid; version: uint; client: ClientResponse/null; items: lista de ItemResponse; status: OrderStatus; descount, increase, grossValue e total: decimal; createdAt: DateTime |

Cliente não contém sua coleção de pedidos, e item não contém o pedido: as respostas não formam ciclos. O GET de pedidos carrega cliente, itens e produtos para montar os DTOs.

## Banco e validação pendente

A migração `20261021180000_DecimalValues` converte seis colunas existentes de double precision para numeric: Products.SuggestedPrice, Orders.Descount/Increase/GrossValue e Items.Quantity/UnitPrice. Snapshot e modelo de destino acompanham a alteração. Não há remoção de tabela/coluna, alteração de nomes ou recálculo de pedidos históricos. OrdersId nullable dos itens passa a ser explícito no modelo; nulabilidade e comportamentos de exclusão existentes são preservados.

A migração foi preparada manualmente e não foi aplicada. Antes de usar este modelo com o banco, validar a conversão em uma cópia isolada: precisão dos dados antigos, valores especiais como NaN/Infinity e valores fora do limite de decimal. Reverter para double pode perder precisão. A nova aplicação depende do alinhamento dessas colunas com o modelo decimal.

A migração `20261022120000_ProtectOrderIntegrity`, também preparada manualmente, substitui as três chaves estrangeiras: cliente → pedidos e produto → itens usam RESTRICT; pedido → itens usa CASCADE. Up e Down não removem tabelas nem recalculam registros históricos. A reversão restaura os comportamentos anteriores de exclusão; não recupera itens de pedidos que tenham sido excluídos. Snapshot e modelo de destino incluem as versões xmin e os novos relacionamentos. Nenhuma migração foi aplicada nesta tarefa.

O controle manual de transações sem inicialização foi removido. Cada operação continua com uma única chamada a SaveChangesAsync e usa a transação padrão do EF Core. IDTO, GetByFilter do repositório e a sobrecarga de Add sem consumidores foram removidos. Autenticação e autorização de acesso permanecem para uma próxima etapa, por decisão do usuário.

A documentação anterior registrava uma compilação em 22/10/2026 com .NET SDK 10.0.401, sem erros nem avisos. Esse resultado antecede as correções de integridade e concorrência. Para o código atual, foi realizada somente revisão estática e conferência do diff; não foram executados builds, testes, chamadas HTTP ou operações no banco, nem criados testes.

Os fluxos acima exigem validação em execução: criação, atualização e consulta das três entidades; pedido com múltiplos itens e produtos repetidos; lista vazia; dados inválidos e itens nulos; cliente/produto inexistentes; overflow; IDs de itens duplicados ou de outro pedido; remoção de itens omitidos; exclusão de pedido com itens; bloqueio da exclusão de clientes/produtos vinculados; versões ausentes e desatualizadas; edições simultâneas, inclusive apenas nos itens com o mesmo total; datas sem fuso e com offset; paginação extrema e ordenação; aplicação/reversão das migrações em banco isolado. Análise estática e compilação não comprovam esses cenários nem o alinhamento do banco com o modelo.
