namespace Ckm
{
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Models;
    using Services;
    using System.Net;
    using System.Threading.Tasks;
    using ViewModels;

    public class Startup
    {
        public static IConfiguration Configuration;

        public static AutoMapper.IMapper Mapper;

        public Startup(IApplicationEnvironment appEnv)
        {
            var builder =
                new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                ;

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

//#if !DEBUG
//                config =>
//                /*
//                config.Filters.Add( new RequireHttpsAttribute())
//                */
//#endif

            // mvc
            services
                .AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver())
                ;

            services.AddIdentity<CkmUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";

                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        // http://wildermuth.com/2015/9/10/ASP_NET_5_Identity_and_REST_APIs
                        // Only called when redirect system 

                        // code is trying to redirect to login e.g. on Html web-page
                        // if api call and that code base returned 200, but not logged in then ...

                        #region 
                        //var isAuthd = ctx.HttpContext.Authentication.
                        // var User = ctx.Request.HttpContext.User; 
                        // var x1 = (User.Identity.IsAuthenticated);
                        #endregion

                        // if { Redirect && an api-call && code base has returned 200 } then ret 401-Unauth
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            // return Unauthorised but not {200, and login-page-html}
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            // redirect to the redirect uri
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    
                        // we are finished with this operation
                        return Task.FromResult(0);
                    }
                };
            })
            .AddEntityFrameworkStores<CkmContext>();

#if DEBUG
            services.AddScoped<IMailService, DebugMailService>();
#else
            services.AddScoped<IMailService, DebugMailService>();
#endif

            // logging
            services.AddLogging();

            services
                .AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<CkmContext>();

            services.AddScoped<CoordService>();
            services.AddTransient<CkmContextSeedData>();
            services.AddScoped<ICkmRepo, CkmRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, CkmContextSeedData seedData, ILoggerFactory loggerFactory)
        {
            //  AutoMapper
            // old way
            AutoMapper.Mapper.Initialize(config =>
                    {
                        config.CreateMap<Trip, TripViewModel>().ReverseMap();
                        config.CreateMap<Stop, StopViewModel>().ReverseMap();
                    });
            // new way v5
            var autoMapperConfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Stop, StopViewModel>().ReverseMap();
                cfg.CreateMap<Trip, TripViewModel>().ReverseMap();
            });
            Mapper = autoMapperConfig.CreateMapper();

            // out of the box either {Console  | DebugWindow }
            // loggerFactory.AddDebug(LogLevel.Information);
            loggerFactory.AddDebug(LogLevel.Warning);

            #region

            // chain of middleware
            //app.UseIISPlatformHandler();

            //app.UseDefaultFiles(); // remove if all routing dome by mvc

            #endregion

            //if (env.IsDevelopment())
            //{
            //    //app.UseBrowserLink();
            //    app.UseDeveloperExceptionPage();
            //    //app.UseDatabaseErrorPage();
            //}

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(config =>
               config.MapRoute(
                   name: "Default",
                   template: "{controller}/{action}/{id?}",
                   defaults: new { controller = "App", action = "Index" }
                   )
            );

            #region

            //app.us
            // app.Run(async (context) =>{});

            #endregion

            await seedData.EnsureSeedDataAsync();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}