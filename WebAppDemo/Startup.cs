using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Learn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAppDemo
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
            // 获得连接字符串
            string connectionString = Configuration.GetConnectionString(Configuration.GetSection("db:ConnectionName")?.Value ?? "DefaultConnection");
            string assemblyFullName = this.GetType().Assembly.FullName;
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assemblyFullName)));

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSwaggerDocument(c =>
            {
                c.Title = "Learn API SwaggerV2";
                c.Version = "V1";
                c.Description = "Learn ASP.NET Core Web API";
                c.DocumentName = "Document_SwaggerV2";
            });

            services.AddOpenApiDocument(c =>
            {
                c.Title = "Learn API OpenApiV3";
                c.Version = "V1";
                c.Description = "Learn ASP.NET Core Web API";
                c.DocumentName = "Document_OpenApiV3";
            });
            services.AddScoped<IGoodsService, GoodsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger().UseSwaggerUi3(c =>
            {
                c.DocExpansion = "list";
                c.DefaultModelExpandDepth = 3;
                c.ValidateSpecification = true;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
