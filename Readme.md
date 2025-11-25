## How to run an application

### 1. Run docker

```shell
docker compose up --build -d
```

There might be an issues with database, so after you see that DB is up and runnig you need to
apply migration.

### 2. Ensure DB applied

```shell
dotnet ef database update `
--project OPS.Data `
--startup-project OPS.WebApi
```

After success migration you would need to restart the postgres container, or you can turn
everything down and apply the same docker command

```shell
docker compose up --build -d
```

After this you should be good to go, please refer to [Swagger](http://localhost:8080/swagger/index.html)

## Design decisions and tradeoffs

1. Decided to use EF Core for data access layer
2. NServiceBus + RabbitMQ for messaging for data consistency
3. We return GUID during order creation and then use it to get order details
   if no order found means that initial validation did not pass and we can refer to logs
   to understand what exactly happened 
4. We use Postgresql for as DB
5. We're not failing order creation if the initial constraints pass (e.g. customer and items exists in db)
but we will not touch the inventory if it is not fulfillable
6. There is a retry mechanism for failed messages, but no implementation for DLQ that can be useful 
in a production version of the app
7. There's a log of total success message processed
8. There's an idempotency check on API layer which is making sure that we won't 
execute the same operation twice, but with existing approach we will generate a new order ID GUID once
the idempotency check pass, this migth be a tricky with updating/cancelling orders and also
with this approach we cannot implement additional check on the rabbitmq layer. This can be reversal, if we pass 
order id as guid and make sure that it's unique during initial check on the rabbitmq layer.

## Assumptions that I've made during this work

1. It can be tricky to spin up a local environment for Postgresql and rabbitmq in docker
2. Sometimes there's a weird library dependency issue, which can be resolved
   via direct reference of the library which is implicitly referenced by other libraries
3. Not sure EF-CodeFirst approach was good here, maybe spinning up DB via SQL scripts would be faster
4. EF handles transactions pretty cool 

### Dev notes (this is just for my copypaste purpose :)

### New migration

```shell
dotnet ef migrations add init `
--project OPS.Data `
--startup-project OPS.WebApi `
--output-dir Migrations 
```

### Drop DP

```shell
dotnet ef database drop `
--project OPS.Data `
--startup-project OPS.WebApi
```

### Remove migration

```shell
dotnet ef migrations remove `
--project OPS.Data `
--startup-project OPS.WebApi
```