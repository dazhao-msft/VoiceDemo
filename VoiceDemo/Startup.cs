using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VoiceDemo.Nexmo;

namespace VoiceDemo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConnections();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseConnections(routes => routes.MapConnectionHandler<NexmoAudioHandler>("/ws/echo"));

            app.UseMvc();
        }
    }
}
