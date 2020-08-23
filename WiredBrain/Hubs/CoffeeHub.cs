using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WiredBrain.Helpers;

namespace WiredBrain.Hubs
{
    /**
     * This class derived from the class Microsoft.AspNetCore.SignalR.Hub
     * Any methods in the hub are meant for clients to call.
     * 
     * ASP.NET Core SignalR integrates with the authentication and authorization
     * you have configured for ASP.NET Core. There is no need for any special configuration
     * for SignalR. Protecting the hub against anonymous access works the same
     * as protecting controllers. Just place the [Authorize] attribute, and 
     * you can also specify authorization policies that applies to this hub.
     * 
     */
    public class CoffeeHub: Hub
    {
        private readonly OrderChecker _orderChecker;

        public CoffeeHub(OrderChecker orderChecker)
        {
            _orderChecker = orderChecker;
        }

        public async Task GetUpdateForOrder(int orderId)
        {
            /**
             * The user can be accessed in the hub by using the Context property.
             * There is a User object there and the object type is ClaimsPrinciple.
             * This is the object used throught ASP.NET Core to represent the user.
             * Among other things, we can access the username for the object by
             * Context.User.Identity.Name or the list of claims the user has by
             * Context.User.Claims. You could use this information for all kinds of 
             * things, for example, you could use user's information to decide
             * in which groups in which groups he should go.
             */
            // Context.User;

            CheckResult result;
            
            do
            {
                // This loop checks if there is a new order
                result = _orderChecker.GetUpdate(orderId);
                Thread.Sleep(1000);
                /**
                 * If there is an update, it waits for a while to simulate the processing
                 * time, and then it notifies the client. There is a Clients property in
                 * the Hub class. With this clients object, we can choose to which clients
                 * you want to send the update. In the below case, we are just notifying
                 * the caller of this hub method.
                 */
                if (result.New)
                    /**
                     * When calling SendAsync, SignalR uses Remote Procedure Call (RPC).
                     * The first parameter of the SendAsync is the name of the function that
                     * will be called on the selected clients. The second one is the parameter
                     * that has to be passed into the function. There are other overloads
                     * in the parameter. SignalR will take care of serializing the parameters
                     * using the configured hub protocols. The default hub protocol is JSON.
                     */
                    await Clients.Caller.SendAsync("ReceiveOrderUpdate", 
                        result.Update);
            } while (!result.Finished); // This loop will end if the order is finished.

            /**
             * If there are no more updates on this function, then we now let the clients
             * call the "Finished" function
             */
            await Clients.Caller.SendAsync("Finished");
        }

        /**
         * Context is the propert in the Hub that gives you user information
         */
        public override async Task OnConnectedAsync()
        {
            /**
             * Everywhere in the hub, you have access to the Context property.
             * ConnectionId is a unique ID of the client that has caused the triggering 
             * of the hub. The connection ID can be used on several options the 
             * Clients property in the hub class offers.
             */

            var connectionId = Context.ConnectionId;

            /**
             * Client option that needs you to specify a a connection Id for the client
             */
            // await Clients.Client(connectionId).SendAsync("NewOrder", order);
            /**
             * AllExcept, which calls a method for all clients except for the client with
             * the specified ID
             */
            // await Clients.AllExcept(connectionId).SendAsync("NewOrder", order);
            /**
             * Groups property in the hub lets you add clients to groups. Again, you need
             * the connectionId for the client you want to add, and the name of the group
             * you want to the add the user to. You can also use Context.User.Claims 
             * to identify which group the user belongs. 
             * The group mechanism in SignalR is very
             * simple. Once you added a client to a new group, the group is created. And,
             * once the last client is removed from the group, the group is destroyed.
             */
            // await Groups.AddToGroupAsync(connectionId, "AmericanoGroup");
            /**
             * In the same way, you can also remove clients from the group. 
             */
            // await Groups.RemoveFromGroupAsync(connectionId, "AmericanoGroup");
            /**
             * You can call methods on all clients in a group by using another options of the
             * hub's clients property. It is simply called Group. Then you can all SendAsync
             * to do an RPC to all clients in that group.
             */
            // await Clients.Group("AmericanoGroup").SendAsync("NewOrder", order);
        }
    }
}
