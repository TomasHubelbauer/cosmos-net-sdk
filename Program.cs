using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace cosmos_net_sdk
{
  class Program
  {
    static async Task Main(string[] args)
    {
      string primaryKey;
      try
      {
        Console.WriteLine("Downloading the primary key from https://localhost:8081/_explorer/quickstart.html…");
        var html = await new HttpClient().GetStringAsync("https://localhost:8081/_explorer/quickstart.html");
        primaryKey = Regex.Match(html, "Primary Key</p>\\s+<input .* value=\"(?<primaryKey>.*)\"").Groups["primaryKey"].Value;
        Console.WriteLine("The primary key has been downloaded.");
      }
      catch
      {
        Console.WriteLine("Failed to download the primary key. Make sure to install and run the Cosmos emulator.");
        Console.WriteLine("The primary key gets downloaded from https://localhost:8081/_explorer/quickstart.html");
        return;
      }

      var client = new DocumentClient(new Uri("https://localhost:8081"), primaryKey);

      await client.CreateDatabaseIfNotExistsAsync(new Database
      {
        Id = nameof(cosmos_net_sdk)
      });

      await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(nameof(cosmos_net_sdk)), new DocumentCollection
      {
        Id = nameof(cosmos_net_sdk)
      });

      await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(nameof(cosmos_net_sdk), nameof(cosmos_net_sdk)), new
      {
        Id = Guid.NewGuid(),
        FirstName = "Tomas",
        LastName = "Hubelbauer",
      });

      // TODO: Find a better way to recreate the UDF each time, `Replace…` seem to be too complex for now
      try
      {
        await client.DeleteUserDefinedFunctionAsync(UriFactory.CreateUserDefinedFunctionUri(nameof(cosmos_net_sdk), nameof(cosmos_net_sdk), "test"));
      }
      catch (System.Exception)
      {
        Console.WriteLine("UDF didn't exist yet");
      }

      await client.CreateUserDefinedFunctionAsync(UriFactory.CreateDocumentCollectionUri(nameof(cosmos_net_sdk), nameof(cosmos_net_sdk)), new UserDefinedFunction
      {
        Id = "test",
        Body = "function test() { return 'test'; }",
      });

      var results = client.CreateDocumentQuery<dynamic>(
        UriFactory.CreateDocumentCollectionUri(nameof(cosmos_net_sdk), nameof(cosmos_net_sdk)),
        $"SELECT {{ firstName: collection.FirstName, lastName: collection.LastName, id: collection.Id, test: udf.test() }} FROM {nameof(cosmos_net_sdk)} collection");

      foreach (var result in results)
      {
        Console.WriteLine(result);
      }
    }
  }
}
