var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddProject<Projects.WebApplication6>("webapplication6")
    .WithExternalHttpEndpoints()
    .WithReference(cache);

builder.Build().Run();

//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Caching.StackExchangeRedis;

//var builder = DistributedApplication.CreateBuilder(args);

//var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

//var redis = builder.AddRedis("redis")
//                   .WithConnectionString(redisConnectionString);

//var app = builder.AddProject<WebApplication6>()
//                .WithReference(redis)
//                .Build();

//app.Run();
