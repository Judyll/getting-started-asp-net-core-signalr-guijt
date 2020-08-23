using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press a key to start listening..");
            Console.ReadKey();
            /**
             * The API is almost the same as the client library for Javascript.
             */
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:60907/coffeehub")
                /**
                 * This is for supporting MessagePack which comes from the 
                 * SignalR.Protocols.MessagePack nuget package.
                 */
                .AddMessagePackProtocol()
                .Build();

            /**
             * Incoming function calls are handled with the On function on the connetion.
             * Here is one difference, "On" is generic, and we need to specify what
             * type of message we are expecting. So now, the first parameter of the passed
             * in function is of type order. It can also support multiple parameters
             * by specifying multiple generic parameters.
             */
            connection.On<Order>("NewOrder", (order) => 
                Console.WriteLine($"Somebody ordered an {order.Product}"));

            /**
             * Connection is now started by calling StartAsync. If something goes wrong,
             * an exception is thrown but you can catch as normal. Normally, you could
             * await this but, since we are at the main method of the console application,
             * GetAwaiter().GetResult() thing.
             */
            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Listening. Press a key to quit");
            Console.ReadKey();
        }
    }
}
