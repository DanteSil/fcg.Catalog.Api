# CatalogAPI

CRUD de jogos, promoções e biblioteca. A API valida os JWTs emitidos pela UsersAPI. Execute os comandos abaixo no diretório deste serviço.

```powershell
$env:ConnectionStrings__DefaultConnection = 'Host=localhost;Database=fcg_catalog;Username=postgres;Password=<senha>'
$env:Jwt__SecretKey = '<mesma chave configurada na UsersAPI>'
dotnet run
```

A API abre em http://localhost:5102. Use `/swagger` para consultar as rotas e `/health` para verificar que o processo está respondendo. Se sobrescrever `Jwt__Issuer` ou `Jwt__Audience`, use os mesmos valores nos dois serviços.

## Teste do pedido via RabbitMQ

O perfil de desenvolvimento usa o RabbitMQ local. Para outro broker, defina `RabbitMQ__Uri`. A exchange `pedidos` deve estar vinculada à fila `pedidos.faturamento` pela chave `pedido.criado`.

1. Inicie a UsersAPI, a CatalogAPI e a PaymentsAPI com o RabbitMQ rodando.
2. No Swagger do catálogo, autentique-se com o JWT de um usuário comum e chame `POST /api/library/{gameId}` para um jogo existente que ainda esteja fora da biblioteca.
3. Confira o status `202 Accepted` e o JSON com `orderId`, `userId`, `gameId` e `price`. O console da PaymentsAPI deve mostrar o mesmo pedido.

O status `202` indica que o broker confirmou a publicação. O catálogo consome `PaymentProcessedEvent` na fila `pagamentos.catalogo`. Após um resultado `Approved`, confira o jogo em `GET /api/library` com o mesmo JWT. Um resultado `Rejected` deixa a biblioteca intacta. Se um resultado aprovado for entregue novamente, a restrição única de usuário e jogo impede duplicatas.

## Kubernetes

Os manifestos deste serviço ficam em k8s/. O script de implantação, a infraestrutura e as instruções para clonar os cinco repositórios estão em https://github.com/DanteSil/fcg.Infrastructure.
