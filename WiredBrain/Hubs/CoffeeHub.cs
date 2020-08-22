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
    }
}
