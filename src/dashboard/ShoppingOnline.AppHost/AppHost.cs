using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(options => { options.WithImageTag("9.17"); })
    .WithDataVolume();

var shoppingDb = postgres.AddDatabase("shoppingDb");


var webapi = builder.AddProject<Projects.ShoppingOnline_API>("shoppingonline-api")
    .WithReference(shoppingDb)
    .WaitFor(shoppingDb);

var frontend = builder.AddJavaScriptApp("shoppingonline-frontend", "../../frontend")
    .WithPnpm()
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithReference(webapi)
    .WithEnvironment("API_URL", webapi.GetEndpoint("https"))
    .WaitFor(webapi);


builder.Build().Run();