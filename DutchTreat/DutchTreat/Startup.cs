using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace DutchTreat
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // adding identity is the simple and less secure way to handle log in log out, which works fine for the simple websites
            services.AddIdentity<StoreUser, IdentityRole>(cfg =>
                {
                    cfg.User.RequireUniqueEmail = true;
                    cfg.Password.RequireDigit = true;
                }
            ).AddEntityFrameworkStores<DutchContext>();

            // If you have some secure data which should not be vulnerable to update recommended is to use tokens
            // In our case we'll use cookies to authenticate users, but will use token to update data through the api(s)
            services.AddAuthentication()
                    .AddCookie()
                    .AddJwtBearer(cfg =>
                   {
                       cfg.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidIssuer = _config["Tokens:Issuer"],
                           ValidAudience = _config["Tokens:Audience"],
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                       };
                   });

            services.AddDbContext<DutchContext>(cfg =>
            {
                cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
            });


            services.AddAutoMapper();

            services.AddTransient<DutchSeeder>();

            services.AddTransient<IMailService, NullMailService>();
            // support for real email service 

            services.AddScoped<IDutchRepository, DutchRepository>();

            services.AddMvc()
                      .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            //.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseDefaultFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();
            // add authentication before MVC
            app.UseAuthentication();


            app.UseNodeModules(env);
            app.UseMvc(cfg =>
            {
                cfg.MapRoute("Default",
                            "/{controller}/{action}/{id?}",
                            new { controller = "App", Action = "Index" });
            });
        }
    }
}
