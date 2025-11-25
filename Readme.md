# DB
## For me
### 0. New migration
```shell
dotnet ef migrations add init `
--project OPS.Data `
--startup-project OPS.WebApi `
--output-dir Migrations 
```
### 0.1. Drop DP
```shell
dotnet ef database drop `
--project OPS.Data `
--startup-project OPS.WebApi
```

### 0.2 Remove migration

```shell
dotnet ef migrations remove `
--project OPS.Data `
--startup-project OPS.WebApi
```

## 1. Ensure DB applied
```shell
dotnet ef database update `
--project OPS.Data `
--startup-project OPS.WebApi
```
