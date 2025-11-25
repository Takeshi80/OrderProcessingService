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
2. NServiceBus for messaging for data consistency
3. We return GUID during order creation and then use it to get order details
   if no order found means -> something went wrong, and we can refer to logs
   to understand what exactly happened (not the best approach)
4. We use Postgresql for local development
5. We're failing whole order creation if any of the constraints are not met. 
This can be expanded. For example, we can create an order with failed status, 
without decreasing the inventory and figure out some approach of re-processing
failed order
6. There is a retry mechanism for failed messages, but no implementation for DLQ that can be useful 
in a production version of the app


## Assumptions that I've made during this work

1. It can be tricky to spin up a local environment for Postgresql and rabbitmq in docker
2. Sometimes there's a weird library dependency issue, which can be resolved
   via direct reference of the library which is implicitly referenced by other libraries
3. Not sure EF-CodeFirst approach was good here, maybe spinning up DB via SQL scripts would be faster
4. 

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