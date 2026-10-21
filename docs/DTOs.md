# Contratos da API

Os controllers recebem DTOs de `DTOs/<entidade>/Requests` e os serviços retornam os de `Responses`. As entidades ficam em `Models/Entities`, mantendo o namespace `DotNetFoundations.Models` para preservar sua identidade no EF. As conversões são explícitas nos serviços; o repositório continua recebendo entidades.

## Operações disponíveis e preparadas

| Entidade | Criação | Consulta | Atualização preparada | Exclusão preparada | Resposta |
| --- | --- | --- | --- | --- | --- |
| Clientes | CreateClientRequest | QueryClientRequest | UpdateClientRequest | DeleteClientRequest | ClientResponse |
| Produtos | CreateProductRequest | QueryProductRequest | UpdateProductRequest | DeleteProductRequest | ProductResponse |
| Pedidos | CreateOrderRequest | QueryOrderRequest | UpdateOrderRequest | DeleteOrderRequest | OrderResponse |
| Itens | CreateItemRequest | QueryItemRequest | UpdateItemRequest | DeleteItemRequest | ItemResponse |

Continuam disponíveis apenas POST e GET em `/api/Clients`, `/api/Products` e `/api/Orders`. Itens são enviados dentro de pedidos; não existe controller de itens. Atualização, exclusão e consulta independente de itens possuem somente contratos, sem novas rotas ou implementação de operações.

Atualização representa substituição completa dos campos editáveis: não é PATCH. Contém os campos de criação e o identificador obrigatório (`clientsId`, `productsId`, `ordersId` ou `itemsId`). Em UpdateOrderRequest, cada item usa UpdateItemRequest e informa `itemsId`. Exclusão contém somente o identificador obrigatório. Esses contratos ainda não podem ser usados por HTTP.

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

Os três DTOs de consulta usados pelos GETs são convertidos para os filtros internos existentes. Todos incluem page (padrão 1), pageSize (padrão 20), orderBy (nome de propriedade persistida) e descending (padrão false). Mantém-se a normalização de page para no mínimo 1 e pageSize para o intervalo 1–100.

| Consulta | Filtros específicos opcionais |
| --- | --- |
| QueryClientRequest | name, nameContains, email, emailContains, phone: string |
| QueryProductRequest | name, nameContains: string; minSuggestedPrice: decimal >= 0 |
| QueryOrderRequest | clientId: Guid; status: OrderStatus; descount, increase, grossValue: decimal >= 0; createdAt: DateTime |
| QueryItemRequest (sem rota) | ordersId, productId: Guid |

Exemplos: `GET /api/Products?NameContains=exemplo&Page=1&PageSize=20` e `GET /api/Orders?Status=0&OrderBy=CreatedAt&Descending=true`. CreatedAt mantém o filtro pelo dia e as regras de data existentes.

GET preserva os envelopes `{ "clients": [] }`, `{ "products": [] }` e `{ "orders": [] }`.

| Resposta | Campos |
| --- | --- |
| ClientResponse | clientsId: Guid; name: string; email e phone: string/null; createdAt: DateTime |
| ProductResponse | productsId: Guid; name: string; suggestedPrice: decimal; createdAt: DateTime |
| ItemResponse | itemsId: Guid; ordersId: Guid/null; productId: Guid; product: ProductResponse/null; quantity, unitPrice e total: decimal; createdAt: DateTime |
| OrderResponse | ordersId e clientId: Guid; client: ClientResponse/null; items: lista de ItemResponse; status: OrderStatus; descount, increase, grossValue e total: decimal; createdAt: DateTime |

Cliente não contém sua coleção de pedidos, e item não contém o pedido: as respostas não formam ciclos. O GET de pedidos carrega cliente, itens e produtos para montar os DTOs.

## Banco e validação pendente

A migração `20261021180000_DecimalValues` converte seis colunas existentes de double precision para numeric: Products.SuggestedPrice, Orders.Descount/Increase/GrossValue e Items.Quantity/UnitPrice. Snapshot e modelo de destino acompanham a alteração. Não há remoção de tabela/coluna, alteração de nomes ou recálculo de pedidos históricos. OrdersId nullable dos itens passa a ser explícito no modelo; nulabilidade e comportamentos de exclusão existentes são preservados.

A migração foi preparada manualmente e não foi aplicada. Antes de usar este modelo com o banco, validar a conversão em uma cópia isolada: precisão dos dados antigos, valores especiais como NaN/Infinity e valores fora do limite de decimal. Reverter para double pode perder precisão. A nova aplicação depende do alinhamento dessas colunas com o modelo decimal.

Foi feita apenas revisão estática. Compilação, testes e chamadas HTTP não foram executados. Permanecem para validação autorizada: criação e consulta das três entidades; pedido com múltiplos itens e produtos repetidos; lista vazia; dados inválidos e itens nulos; cliente/produto inexistentes; overflow; paginação e ordenação; aplicação/reversão da migração em banco isolado.
