# AzureFunctions.ModelBinding
ModelBinding for Azure Functions



## Why


## Usage

```shell
Install-Package AzureFunctions.ModelBinding -Version 1.0.0
```

or search for `AzureFunctions.ModelBinding`



### 1. Register the service in your Startup.cs
```csharp
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.ModelBinding;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //... 
            //your other registrations here
            
            ModelBinder.SerializationSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            ModelBinder.Register<YourModel>();
            ModelBinder.Register<AnotherModel>();
        }
    }

```
> Create a Startup.cs if you don't have one.

### 2. Add your schema under: /Schemas/

should conform to JSON Schema [specs](https://jsonschema.net)
```json
{

  "Values": {

    "ConnectionString": "MyConnectionStringWithCredentials",
    "Database":  "MyDatabaseName"
  }
}
```

### 3. Inject and use it in your function

Inject the client in the constructor. Save it to a static variable, and use it normally.

```csharp
namespace MyNamespace
{
    public class MyFunctionName
    {

        [FunctionName("TestClient")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "yourResource")] HttpRequest req,
            ILogger log
            )
        {
            var binding = ModelBinder.Bind<YourModel>(req);

            if (!binding.IsValid)
                return binding.BadRequest;  //this will return any issues it found against the schema

            binding.Model //enjoy your model

            return new OkObjectResult(binding.Model);

        }
    }
}
```
