using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WiredBrain.Helpers;
using WiredBrain.Hubs;
using WiredBrain.Models;

namespace WiredBrain.Controllers
{
    [Route("[controller]")]
    public class CoffeeController : Controller
    {
        private readonly IHubContext<CoffeeHub> _coffeeHub;

        public CoffeeController(IHubContext<CoffeeHub> coffeeHub)
        {
            _coffeeHub = coffeeHub;
        }

        /**
         * This is a rest-style method which gets called when somebody hits
         * the order button on the webpage. The call happens with the regular
         * AJAX call
         */
        [HttpPost]
        public async Task<IActionResult> OrderCoffee(
            [FromBody]Order order)
        {
            /**
             * We all want all connected clients to know that a new order
             * has been placed. Note that we don't have the context of the caller
             * when injecting a hub like this. A caller option on clients is missing
             * and also any other options that has to do with the caller. Maybe a better
             * alternative to this approach is maybe to just place this OrderCoffee method
             * in the hub, and let the client call that using SignalR instead of regular
             * AJAX call. We did not choose that approach in this case so that we can show
             * that we can access hubs from everywhere.
             */
            await _coffeeHub.Clients.All.SendAsync("NewOrder", order);
            //Save order somewhere and get order id
            return Accepted(1); //return order id
        }
    }
}
