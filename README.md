# Cosmos .NET SDK

[**WEB**](https://tomashubelbauer.github.io/cosmos-net-sdk)

1. `dotnet new console`
2. `dotnet add package Microsoft.Azure.DocumentDB` (2.3.0)
3. `using Microsoft.Azure.Documents;`
4. Error. WTF?
5. https://github.com/Azure/azure-cosmos-dotnet-v2/issues/596
6. `dotnet add package Microsoft.Azure.DocumentDB`
7. `dotnet add package Microsoft.Azure.DocumentDB.Core` (2.3.0)
8. `<LangVersion>latest</LangVersion>`
9. Download `primaryKey` and instantiate `client`
10. https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-use-stored-procedures-triggers-udfs#udfs
