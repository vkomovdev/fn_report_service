using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using ReportService.Options;
using ReportService.Domain;

namespace ReportService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                // Disable endpoint routing to allow UseMvc compatibility
                options.EnableEndpointRouting = false;
            });

            services.Configure<EmpCodeServiceOptions>(Configuration.GetSection("EmpCodeService"));
            services.Configure<SalaryServiceOptions>(Configuration.GetSection("SalaryService"));

            services.AddHttpClient<IEmpCodeResolver, EmpCodeResolver>((sp, http) =>
            {
                var o = sp.GetRequiredService<IOptions<EmpCodeServiceOptions>>().Value;
                http.BaseAddress = o.BaseUrl;
            });
            services.AddHttpClient<ISalaryProvider, SalaryProvider>((sp, http) =>
            {
                var o = sp.GetRequiredService<IOptions<SalaryServiceOptions>>().Value;
                http.BaseAddress = o.BaseUrl;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
