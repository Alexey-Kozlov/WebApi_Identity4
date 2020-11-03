using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Services.Authentication;
using WebApi.Repositories.Users;
using WebApi.Logger;
using WebApi.Filters;

namespace WebApi
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
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration.GetValue<string>("AuthenticationServerUrl");
                options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;
                //options.RequireHttpsMetadata = true;
                options.RequireHttpsMetadata = false;
                options.LegacyAudienceValidation = false;
                options.ApiName = Configuration.GetValue<string>("ApiName");                
                options.JwtBearerEvents = new JwtBearerEvents
                {
                    OnMessageReceived = MessageReceivedAsync
                };
            });
            services.AddControllersWithViews(p =>
            {
                p.Filters.Add(typeof(ControllerRequestActionLogger));
                p.Filters.Add(typeof(ExceptionFilter));
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(Configuration.GetValue<string>("AllowedCorsUrls").Split(','))
                        .WithHeaders("*")
                        .WithMethods("*")
                        .AllowCredentials());
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("roles", "Admin"));
                options.AddPolicy("User", policy => policy.RequireClaim("roles", "User"));
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IHttpLogHandlerService, HttpLogHandlerService>();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ControllerResponseActionLogger>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private Task MessageReceivedAsync(MessageReceivedContext context)
        {
            
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaders) || StringValues.IsNullOrEmpty(authHeaders))
            {
                return Task.CompletedTask;
            }

            var token = authHeaders[0].Replace("Bearer ", string.Empty);

            JwtSecurityToken jwtToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            }

            if (jwtToken == null)
            {
                return Task.CompletedTask;
            }

            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
            return Task.CompletedTask;
        }


    }
}
