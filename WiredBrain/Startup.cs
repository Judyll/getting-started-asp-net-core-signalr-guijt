using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WiredBrain.Helpers;
using WiredBrain.Hubs;

namespace WiredBrain
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(new Random());
            services.AddSingleton<OrderChecker>();
            services.AddHttpContextAccessor();
            /**
             * Adding the needed types to the dependency injection container.
             * You can use AddSignalR() to use the default or you can use a lamba
             * to fine tune the configuration like AddSignalR(r => r.EnableDetailedErrors)
             * 
             * The .AddMessagePackProtocol allows us to support MessagePacks. The server
             * will now support message pack in addition to the JSON support. 
             * This comes from the SignalR.Protocols.MessagePack nuget package
             */
            services.AddSignalR().AddMessagePackProtocol();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            /**
             * We also need to plug SignalR into the pipeline. As a parameter, we have to
             * provide a function that gets a HubRouteBuilder object. Just like MVC,
             * we have to setup routing. In this case not for controllers, but for hubs.
             * We are mapping the class CoffeeHub to /coffeehub. We are going to need this
             * relative URL when we are going to write the client part in wiredbrain.js.
             */
            app.UseSignalR(routes => routes.MapHub<CoffeeHub>("/coffeehub"));
            app.UseMvc();
        }
    }
}
