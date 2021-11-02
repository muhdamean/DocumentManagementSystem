using DocumentManagementSystem.Models;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            Env = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //heroku 
            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddDbContextPool<AppDbContext>(option => option.UseNpgsql(Configuration.GetConnectionString("DMSCon"), b => b.MigrationsAssembly("DocumentManagementSystem")).UseLowerCaseNamingConvention());
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                
                //options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            //.AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            //.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            var builder = services.AddControllersWithViews();
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddHttpContextAccessor();
            services.AddScoped<IMailService, MailService>();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/home/signin");
                options.LogoutPath = new PathString("/home/index");
                options.AccessDeniedPath = new PathString("/admin/accessdenied");
            });

            services.AddAuthorization(options =>
            {
                //Claims Policy
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));
               
                //options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequiement())); //custom auth with no personal role edit

                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context=>context.User.IsInRole("Admin")&& 
                //                                                                        context.User.HasClaim(claim=>claim.Type=="Edit Role" && claim.Value=="true")||
                //                                                                        context.User.IsInRole("Super Admin"))); //both (Admin && Edit Role) || Super Admin
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));

                //Role Policy
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin", "Super Admin", "HOD", "User")); // dot RequireRole("Admin","User","Manager") for multiple roles in the policy
            });
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
                options.AppendTrailingSlash = true;
            });
           
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
                if (env.IsProduction())
                {
                    app
                        .UseForwardedHeaders()
                        .UseHttpsRedirection();
                    app.UseDeveloperExceptionPage();
                }
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            RotativaConfiguration.Setup((Microsoft.AspNetCore.Hosting.IHostingEnvironment)env);
        }
    }
}
