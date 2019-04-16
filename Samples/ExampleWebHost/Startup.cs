using LiquidProjections;
using LiquidProjections.ExampleWebHost;
using LiquidProjections.Statistics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleWebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var eventStore = new JsonFileEventStore("ExampleEvents.zip", 100);
            var projectionsStore = new InMemoryDatabase();

            var dispatcher = new Dispatcher(eventStore.Subscribe);
            var stats = new ProjectionStats(() => System.DateTime.UtcNow);
            var bootstrapper = new CountsProjector(dispatcher, projectionsStore, stats);

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<InMemoryDatabase>(projectionsStore);
            services.AddSingleton<ProjectionStats>(stats);

            bootstrapper.Start();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
