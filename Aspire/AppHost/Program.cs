var builder = DistributedApplication.CreateBuilder(args);

// Modern Aspire 9.3 approach - use parameters with secret flag
var dbUsername = builder.AddParameter("DbUsername", secret: false);
var dbPasswd = builder.AddParameter("DbPasswd", secret: true);

// NEW: Admin credentials parameters - secure and hidden from Dashboard
var adminUsername = builder.AddParameter("AdminUsername", "admin", secret: false);
var adminEmail = builder.AddParameter("AdminEmail", "admin@tabula.local", secret: false);
var adminPassword = builder.AddParameter("AdminPassword", secret: true);

// NEW: JWT configuration parameters - fully integrated with Aspire 9.3
var jwtKey = builder.AddParameter("JwtKey", secret: true);

var docker = builder.AddDockerComposeEnvironment("tabula-env")
    .WithProperties(env =>
    {
        env.BuildContainerImages = true; // Build images locally
    })
    .ConfigureComposeFile(file =>
    {
        file.Name = "tabulaCompose"; // Custom compose file name
    });

#pragma warning disable ASPIRECOMPUTE001
var postgres = builder.AddPostgres("postgres")
    .WithUserName(dbUsername)
    .WithPassword(dbPasswd)
    .WithPgAdmin()
    .PublishAsDockerComposeService((_, service) =>
    {
        // Custom Docker Compose configuration
        service.Labels["app"] = "tabula";
        service.Labels["service"] = "database";
        service.Restart = "always";
        service.AddEnvironmentalVariable("POSTGRES_DB", "tabula_main");
    })
    .WithComputeEnvironment(docker);

var tabulaDb = postgres.AddDatabase("TabulaDb");
var identityDb = postgres.AddDatabase("IdentityDb");

builder.AddProject<Projects.Presentation>("apiservice")
    .WithReference(tabulaDb)
    .WithReference(identityDb)
    .WaitFor(tabulaDb)
    .WaitFor(identityDb)
    .WithEnvironment("ADMIN_USERNAME", adminUsername)
    .WithEnvironment("ADMIN_EMAIL", adminEmail)
    .WithEnvironment("ADMIN_PASSWORD", adminPassword)
    .WithEnvironment("JwtSettings__Key", jwtKey)
    .PublishAsDockerComposeService((_, service) =>
    {
        service.Labels["app"] = "tabula";
        service.Labels["service"] = "api";
        service.Restart = "unless-stopped";
        service.AddEnvironmentalVariable("ASPNETCORE_ENVIRONMENT", "Production");
    })
    .WithComputeEnvironment(docker);

builder.Build().Run();
