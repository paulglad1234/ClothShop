using Autofac;
using Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shop
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    //options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/user/login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/user/login");
                });
            // services.AddAuthentication("AdminAuthentication")
            //     .AddCookie(options => //CookieAuthenticationOptions
            //     {
            //         options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/admin/login");
            //         options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/user/login");
            //     });
            services.AddControllersWithViews();
            services.AddSwaggerGen();
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(dbContext => new ShopContext(Configuration.GetConnectionString("Debug")));
            builder.RegisterModule<AppModule>();
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
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Catalog}/{action=Index}");
                endpoints.MapControllers();
            });
        }
    }
}
