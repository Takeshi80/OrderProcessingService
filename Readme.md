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

After this you should be good to go, please refer to [Swagger](http://localhost:8080/swagger/index.html).
You can use API to fetch existing items/customers/inventory to simulate
different scenarios.

## Design decisions and tradeoffs

## Infra
1. Decided to use EF Core with a code-first approach 
to have a simple and straightforward way to create DB schema.
2. Postgresql DB for the data access layer (because it's easy to spin up in docker)
3. NServiceBus + RabbitMQ for messaging & data consistency

## Logic (and some assumptions)
1. To have instant API response, we're publishing the message into RabbitMQ
and returning the GUID representation of the order for the client.
With this GUID we can call GET API to observe the status.
2. Order will be created despite any validation errors.
In case of failure, there's FailureReason on the GET Order API response.
We can also refer to app logs to understand in more detail what happened.
3. Orders with failed status will not affect inventory
4. There is a retry mechanism for failed messages, but no implementation for DLQ that can be useful 
in a production version of the app
5. There's a log of a total success message processed via OPS.Processor
6. There's an idempotency check on the API layer that is making sure that we won't 
execute the same operation twice. However, with the existing approach we will generate a new order ID GUID once
the idempotency check passes. Also, with this approach we cannot implement additional check on the rabbitmq layer. 
It can be implemented differently, for example, we can pass order id
as guid and make sure that it's unique during the initial check on the rabbitmq layer.
7. Our OPS.Processor is a simple console app that runs in the background and processes messages from RabbitMQ,
but there's only 1 handler with only 1 service which is actually doing everything. 
In ideal world we can split it into multiple services and can add additional rabbitmq communication
to initiate some chains of messages (e.g. Validation & order Creation -> Discount calculation -> Inventory calculation).
Basically after order is created we can fire a few rabbitmq messages to make discount and inventory calculation in parallel.

## Assumptions that I've made during this work

1. It can be tricky to spin up a local environment for Postgresql and rabbitmq in docker
2. Sometimes there's a weird library dependency issue, which can be resolved
   via direct reference of the library which is implicitly referenced by other libraries
3. Not sure EF-CodeFirst approach was good here, maybe spinning up DB via SQL scripts would be faster
4. EF handles transactions pretty cool
5. EF limits parallel execution on the same DB context (I was thinking to implement this for validation),
there's a way to make it work, but require some additional work

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