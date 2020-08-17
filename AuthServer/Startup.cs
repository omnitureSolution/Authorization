using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AuthServer.Persistence;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace AuthServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddControllersWithViews();
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddInMemoryClients(Config.GetClients())
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    .AddTestUsers(TestUsers.Users);

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var builder = services.AddIdentityServer()
                  //.AddInMemoryIdentityResources(Config.IdentityResources)
                  //.AddInMemoryClients(Config.GetClients())
                  //.AddInMemoryApiScopes(Config.GetApiScopes())
                  //.AddDeveloperSigningCredential()
                  //.AddTestUsers(TestUsers.Users)
                  .AddConfigurationStore(options =>
                  {
                      options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                          sql => sql.MigrationsAssembly(migrationsAssembly));
                  })
                  .AddOperationalStore(options =>
                  {
                      options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                          sql => sql.MigrationsAssembly(migrationsAssembly));
                  });

            //if (CurrentEnvironment.IsDevelopment())
            //{
            //    builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            var cert = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "IdsvCert.pfx"), "Admin123");
            builder.AddSigningCredential(cert);
            //}

            services.AddTransient<IUserStore, UserStore>();
            services.AddTransient<IProfileService, UserProfileService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (context.Clients.Any())
                    foreach (var client in context.Clients)
                    {
                        context.Clients.Remove(client);
                    }
                context.SaveChanges();
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();

                if (context.IdentityResources.Any())
                    foreach (var resource in context.IdentityResources)
                    {
                        context.IdentityResources.Remove(resource);
                    }

                context.SaveChanges();

                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();

                if (context.ApiScopes.Any())
                    foreach (var scope in context.ApiScopes)
                    {
                        context.ApiScopes.Remove(scope);
                    }

                context.SaveChanges();
                foreach (var resource in Config.GetApiScopes())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();

                if (context.ApiResources.Any())
                    foreach (var contextIdentityResource in context.ApiResources)
                    {
                        context.ApiResources.Remove(contextIdentityResource);
                    };
                context.SaveChanges();
                foreach (var resource in Config.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();

            }
        }
    }
}
