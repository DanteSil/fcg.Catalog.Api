# CatalogAPI

CRUD de jogos, promoções e biblioteca. A API valida os JWTs emitidos pela UsersAPI. Execute os comandos abaixo no diretório deste serviço.

```powershell
$env:ConnectionStrings__DefaultConnection = 'Host=localhost;Database=fcg_catalog;Username=postgres;Password=<senha>'
$env:Jwt__SecretKey = '<mesma chave configurada na UsersAPI>'
dotnet run
```

A API abre em http://localhost:5102. Use `/swagger` para consultar as rotas e `/health` para verificar que o processo está respondendo. Se sobrescrever `Jwt__Issuer` ou `Jwt__Audience`, use os mesmos valores nos dois serviços.
