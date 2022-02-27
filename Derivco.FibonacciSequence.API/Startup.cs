using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Derivco.FibonacciSequence.API.Filters;
using Derivco.FibonacciSequence.API.Middleware;
using Derivco.FibonacciSequence.Logic;
using Derivco.FibonacciSequence.Logic.Cache;
using Derivco.FibonacciSequence.Logic.MemoryService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Derivco.FibonacciSequence.API
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
            services.AddTransient<IFibonacciSequenceService, FibonacciSequenceService>();
            
            services.AddScoped(typeof(IMemoryCacheService<,>), typeof(MemoryCacheService<,>));
            services.AddTransient<IFibonacciSequenceMemoryCacheService, FibonacciSequenceMemoryCacheService>();
            
            services.AddTransient<IMemoryService, MemoryService>();
            
            services.AddScoped<RequestValidationFilterAttribute>();
            
            services.AddMemoryCache();
            
            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Derivco Test Task",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Aleksander Gordeev",
                        Email = "gordeyev90@icloud.com"
                    }
                });
                var currentAssembly = Assembly.GetExecutingAssembly();  
                
                var xmlDocs = currentAssembly.GetReferencedAssemblies()  
                    .Union(new AssemblyName[] { currentAssembly.GetName()})  
                    .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))  
                    .Where(f=>File.Exists(f)).ToArray();
                
                Array.ForEach(xmlDocs, (d) =>  
                {  
                    c.IncludeXmlComments(d);  
                });  
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Derivco.FibonacciSequence.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.ConfigureCustomExceptionMiddleware();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}