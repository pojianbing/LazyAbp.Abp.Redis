using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace sample.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<sampleWebModule>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeApplication();
        }
    }
}
