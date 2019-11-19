using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankOfDotNet.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace BankOfDotNet.API
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
            services.AddControllers();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.ApiName = "bankOfDotNetApi";
            });

            services.AddDbContext<BankContext>(options => options.UseInMemoryDatabase("BankingDb"));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "BankOfDotNetAPI",
                    Version = "v1"
                });

                options.OperationFilter<CheckAuthorizeOperationFilter>();

                options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
                    {
                        Implicit = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("http://localhost:5000/connect/token"),
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize"),
                            Scopes = new Dictionary<string, string> { { "bankOfDotNetApi", "Customer API for BankOfDotNet" } }
                        }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "BankOfDotNet API V1");
                options.OAuthClientId("bankOfDotNetApi");
                options.OAuthAppName("Customer API for BankOfDotNet");
            });
        }
    }

    internal class CheckAuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for Any existing Authorize Attribute
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            operation.Security.Add(new OpenApiSecurityRequirement 
            {
                 
            });
        }
    }
}
