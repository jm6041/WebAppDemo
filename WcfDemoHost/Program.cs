using Autofac;
using Autofac.Extensions.DependencyInjection;
using Learn.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WcfServices;
using System.ServiceModel;
using Autofac.Integration.Wcf;

namespace WcfDemoHost
{
    public class ServiceCollectionFactory
    {
        public static readonly IServiceCollection ServiceCollection = new ServiceCollection();
    }

    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", true, true);
            IConfiguration configuration = configBuilder.Build();

            IServiceCollection services = ServiceCollectionFactory.ServiceCollection;
            services.AddOptions();
            services.AddLogging((b) =>
            {
                b.AddConfiguration(configuration.GetSection("Logging"));
                b.AddConsole();
                b.AddDebug();
            });

            // 获得连接字符串
            string connectionString = configuration.GetConnectionString(configuration.GetSection("db:ConnectionName")?.Value ?? "DefaultConnection");
            string assemblyFullName = typeof(Program).Assembly.FullName;
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assemblyFullName)));
            services.AddScoped<IGoodsService, GoodsService>();
            services.AddScoped<IGoodsOutService, GoodsOutService>();

            services.AddAutofac();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            IServiceProvider serviceProvider = new AutofacServiceProvider(container);
            ContainerBuilder builder = new ContainerBuilder();
            try
            {
                ServiceHost hostValue = new ServiceHost(typeof(GoodsOutService));
                hostValue.AddDependencyInjectionBehavior<IGoodsOutService>(container);
                hostValue.Open();

                Console.WriteLine(nameof(GoodsOutService) + "已经启动！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
