using CasaDoCodigo.Areas.Carrinho.Data;
using CasaDoCodigo.Areas.Catalogo.Data;
using CasaDoCodigo.Areas.Catalogo.Data.Repositories;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Serilog;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace CasaDoCodigo
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddDistributedMemoryCache();
            services.AddSession();

            ConfigurarContexto<ApplicationContext>(services, "Default");
            ConfigurarContexto<CatalogoDbContext>(services, "Catalogo");

            ConfigurarRedis(services);
            ConfigurarDI(services);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication();
        }

        private static void ConfigurarDI(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IHttpHelper, HttpHelper>();
            services.AddTransient<ILivroService, LivroService>();            
            //services.AddTransient<IProdutoRepository, EFProdutoRepository>();
            services.AddSingleton<IProdutoRepository, ElasticProdutoRepository>();
            services.AddTransient<ICarrinhoRepository, RedisCarrinhoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
        }

        private void ConfigurarRedis(IServiceCollection services)
        {
            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(Configuration.GetConnectionString("RedisConnectionString"), true);
                configuration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(configuration);
            });
        }

        private void ConfigurarContexto<T>(IServiceCollection services, string nomeConexao) where T : DbContext
        {
            string connectionString = Configuration.GetConnectionString(nomeConexao);

            services.AddDbContext<T>(options =>
                options.UseSqlServer(connectionString)
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            InitializeProductRepository<IProdutoRepository>(app);
            MigrateDatabase<ApplicationContext>(app);
            //MigrateDatabase<CatalogoDbContext>(app);
            
            _loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "AreaCatalogo",
                    areaName: "Catalogo",
                    template: "Catalogo/{controller=Home}/{action=Index}/{pesquisa?}");

                routes.MapAreaRoute(
                    name: "AreaCarrinho",
                    areaName: "Carrinho",
                    template: "Carrinho/{controller=Home}/{action=Index}/{codigo?}");

                routes.MapAreaRoute(
                    name: "AreaCadastro",
                    areaName: "Cadastro",
                    template: "Cadastro/{controller=Home}/{action=Index}");

                routes.MapAreaRoute(
                    name: "AreaPedido",
                    areaName: "Pedido",
                    template: "Pedido/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{codigo?}");
            });
        }

        private static void MigrateDatabase<T>(IApplicationBuilder app) where T : DbContext
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<T>())
                {
                    context.Database.Migrate();
                }
            }
        }

        private static void InitializeProductRepository<T>(IApplicationBuilder app) where T : IProdutoRepository
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var repository = serviceScope.ServiceProvider.GetService<T>();
                repository.Initialize();
            }
        }
    }
}
