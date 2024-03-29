﻿using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using KiteHelper.Filters;
using KiteHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
namespace KiteHelper
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Serilog Configuration

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithCorrelationId()
                .Enrich.WithSensitiveDataMasking()
                .Enrich.WithProperty("AssemblyName", Assembly.GetExecutingAssembly().GetName().Name)
                .WriteTo.Console()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles", LogEventLevel.Warning)
                .CreateLogger();

            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("MyCors",
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .WithMethods("GET", "POST", "PUT", "DELETE");
                    });
            });

            services.AddDbContext<DatabaseContext>(
                options => options
                    .UseInMemoryDatabase("KiteHelperDatabase", inMemoryOptions =>
                    {
                        inMemoryOptions.EnableNullChecks(true);
                    })
                , ServiceLifetime.Transient);

            services.AddControllersWithViews()
                .AddJsonOptions(jsonOptions => { });

            services
                .AddControllers(options =>
                {
                    options.Filters.Add<HttpResponseExceptionFilter>();
                })
                .AddJsonOptions(jsonOptions => { })
                .ConfigureApiBehaviorOptions(x => { x.SuppressModelStateInvalidFilter = true; }
                );

            services.AddValidatorsFromAssemblyContaining<Startup>();
            services.AddFluentValidationAutoValidation();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetExecutingAssembly().GetName().Name, Version = "v1" });
            });
            //services.AddEndpointsApiExplorer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHsts();

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetExecutingAssembly().GetName().Name));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("MyCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapFallbackToFile("index.html");
            });




        }
    }
}
