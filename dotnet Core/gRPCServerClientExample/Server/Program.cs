// Created using the gRPC template in Visual Studio

// References:
// https://docs.microsoft.com/en-us/aspnet/core/grpc/?view=aspnetcore-6.0
// https://www.youtube.com/watch?v=ru5x_hDZ9Qw


using GrpcServiceExample.Services;

namespace Company.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // Adding in the gRPC service implies a whole load of "default" settings such as endpoints to listen on etc.
            // Some people call this an "opinionated" service.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}