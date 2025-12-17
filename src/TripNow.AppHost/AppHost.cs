var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithImageTag("15-alpine")
                      .WithDataVolume();

var db = postgres.AddDatabase("tripnow-db");

builder.AddProject<Projects.TripNow_Api>("api")
       .WithReference(db);

builder.Build().Run();
