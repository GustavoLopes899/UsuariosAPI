using Backend.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace Backend
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MeuDbContext>(options =>
            {
                string tns = Configuration.GetConnectionString("DefaultConnection");
                string usuario = Configuration["ConnectionStrings:User"];
                string senha = Configuration["ConnectionStrings:Password"];
                options.UseOracle($"User Id={usuario};Password={senha};Data Source={tns};");
            });

            this.ConfigurarServicosSwagger(services);
            services.AddCors();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Usuarios API");
                });
            }

            ILogger logger = loggerFactory.CreateLogger<Startup>();
            FeatureCollection features = app.Properties["server.Features"] as FeatureCollection;
            logger.LogInformation("Servidor iniciado com sucesso!");
            foreach (string endereco in features.Get<IServerAddressesFeature>().Addresses)
            {
                logger.LogInformation("Escutando em: " + endereco);
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void ConfigurarServicosSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Usuarios API",
                    Version = "v1",
                    Description = "ASP.Net Core API para cadastro de usu�rios e seus dependentes",
                    Contact = new OpenApiContact
                    {
                        Name = "Gustavo Lopes",
                        Email = "GustavoLopes899@gmail.com",
                        Url = new Uri("https://github.com/GustavoLopes899"),
                    }
                });
                string arquivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string pastaXml = Path.Combine(AppContext.BaseDirectory, arquivoXml);
                options.IncludeXmlComments(pastaXml);
            });
        }
    }
}