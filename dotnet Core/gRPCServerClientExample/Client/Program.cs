using Grpc.Net.Client;
using static GrpcServiceExample.Greeter;
using GrpcServiceExample;

namespace gRPCclient
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            // The channel represents the location of the running gRPC endpoint.
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            
            // The client is the local representation of the remote service.
            var client = new GreeterClient(channel);

            HelloRequest req = new HelloRequest();
            req.Name = "Bob";

            HelloReply response = client.SayHello(req);

            Console.WriteLine(response.Message);
            Console.WriteLine("Request Name length was " + response.Length + " characters long.");

        }
    }
}