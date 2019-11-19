using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankOfDotNet.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false).Build();

            string connectionString = config.GetSection("connectionString").Value;
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.GetAllApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddTestUsers(Config.GetTestUsers());
                // Configuration Store: store clients and resources
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //         builder.UseSqlServer(connectionString,
                //         sql => sql.MigrationsAssembly(migrationAssembly));
                //})
                //// operational Store will store tokens, consents, codes
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //         builder.UseSqlServer(connectionString,
                //         sql => sql.MigrationsAssembly(migrationAssembly));

                //    // this enables automatic token cleanup. this is optional.
                //    options.EnableTokenCleanup = false;
                //    options.TokenCleanupInterval = 3600;
                //}
                

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            InitializeIdentityServerDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseIdentityServer();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapDefaultControllerRoute();
            });



        }

        private void InitializeIdentityServerDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            //serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            //context.Database.Migrate();

            //Seed the data.
            foreach (var item in Config.GetClients())
            {
                if (!context.Clients.Any(e => e.ClientId == item.ClientId))
                {
                    context.Clients.Add(item.ToEntity());
                    context.SaveChanges();
                }
            }

            if (!context.IdentityResources.Any())
            {
                context.IdentityResources.AddRange(Config.GetIdentityResources().Select(e => e.ToEntity()));
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                context.ApiResources.AddRange(Config.GetAllApiResources().Select(e => e.ToEntity()));
                context.SaveChanges();
            }

        }
    }
}
