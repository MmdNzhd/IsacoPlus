using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using KaraYadak.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using KaraYadak.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using KaraYadak.Extention;
using KaraYadak.Services;
using Parbad.Builder;

namespace KaraYadak
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
            //set JsonResult For WebApi
            //services.AddControllers(options =>
            //{
            //    options.RespectBrowserAcceptHeader = true; // false by default
            //});
            services.AddResponseCaching();

            services.AddSingleton<HtmlEncoder>(
                 HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin,
                                                           UnicodeRanges.All}));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
              .AddRoleManager<RoleManager<IdentityRole>>()
              .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/";
                options.Cookie.Name = "ProjectCookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.LogoutPath = "/SiteAccount/Logout";
                options.LoginPath = "/SiteAccount/Login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddSession();

            services.AddHttpContextAccessor();

            //services.AddControllersWithViews();
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(new ModelStateCheckFilter());
                //options.Filters.Add(new ClaimsAuthorizeAttribute());

            });
            services.AddRazorPages()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/{0}.cshtml");
                });


            services.AddParbad().ConfigureGateways(gateways =>
                                {
                                    //gateways
                                    //    .AddParsian()
                                    //    .WithAccounts(accounts =>
                                    //    {
                                    //        accounts.AddInMemory(account =>
                                    //        {
                                    //            account.LoginAccount = "T46BcVoBu0O4lx7415aa";
                                    //        });
                                    //    });

                                    gateways
                                        .AddParbadVirtual()
                                        .WithOptions(options => options.GatewayPath = "/MyVirtualGateway");
                                })
                        .ConfigureHttpContext(builder => builder.UseDefaultAspNetCore())
                        .ConfigureStorage(builder => builder.UseMemoryCache());

            //services.AddScoped<IActionContextAccessor, ActionContextAccessor>();
            //services.AddScoped<ISiteVisitService, SiteVisitService>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IPaymentService, PaymentService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=HomeSite}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            app.UseMiddleware<SiteVisitCounter>();

        }



    }
}
