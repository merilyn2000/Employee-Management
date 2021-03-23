using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditRolePolicy", policy =>
                    policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())); // review

                //options.InvokeHandlerAfterFailure = false , deoarece daca avem mai multe handle-uri si prima da fail
                //continua a doua (= true by default) !!! Part 103 . Part 102->2 sau mai multe Handle-uri 

                options.AddPolicy("AdminRolePolicy",
                    policy => policy.RequireRole("Admin")
                                    .RequireClaim("Create Role")
                                    .RequireClaim("Delete Role")
                                    .RequireClaim("Edit Role"));
                // se poate sa nu apara deloc edit daca nu ai acces , in loc de accesDeniedView . Part 96
            });

            services.AddSingleton<IAuthorizationHandler,
                 CanEditOnlyOtherAdminRolesAndClaimsHandler>(); // we can add a SuperAdminRole :
                                        //services.AddSingleton<IAuthorizationHandler,SuperAdminRole >

             services.AddMvc(config =>
             {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
             });
                services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "949055707459-f9itlq6mi6u29tbkeavtcs4smpih6m2u.apps.googleusercontent.com";
                options.ClientSecret = "PRRig_YjpbzmZJsXo1kBePol";
            });
            services.AddAuthentication().AddFacebook(options =>
            {
                options.ClientId = "266882011647575";
                options.ClientSecret = "8f1dcf9acd159c5adbd4f989e150ca6a";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
//app.UseMvcWithDefaultRoute();

//app.UseRouting();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/", async context =>
//    {
//        await context.Response.WriteAsync("hi");
//    });
//});