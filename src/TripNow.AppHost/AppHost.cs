using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var tripNowDb = "tripnow-db";

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("15-alpine")
    .WithEnvironment("POSTGRES_DB", tripNowDb)
    .WithEnvironment("POSTGRES_USER", "admin")
    .WithEnvironment("POSTGRES_PASSWORD", "TripNow2025!")
    .WithDataVolume();
var db = postgres.AddDatabase(tripNowDb);


var api = builder.AddProject<Projects.TripNow_Api>("api")
    .WithReference(db)
    .WaitFor(db)
    .WithExternalHttpEndpoints();


builder.Build().Run();
