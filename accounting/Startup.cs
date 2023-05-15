using accounting.src.Data;
using accounting.src.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace accounting
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            string secretKey = jwtSettings.GetValue<string>("Key")!;


            services
                .AddControllers(options => 
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>())
                .AddNewtonsoftJson();

            services.AddEndpointsApiExplorer();


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Accounting Api",
                    Description = "Api for game of mediicine university",
                });

                options.EnableAnnotations();
            });

            services.AddDbContext<AppDbContext>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(
                    options => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    }
                );
            services.AddAuthorization();

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = null;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = null;
            });

        }

        public void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //InitAdmin();


            app.Run();
        }


        private void InitAdmin()
        {
            using var context = new AppDbContext(new DbContextOptions<AppDbContext>(), _config);

            var user = new User
            {
                Email = "admin@okei.com",
                FirstName = "admin",
                LastName = "adminov",
                Patronymic = "adminovich",
                Password = "aaf3cf044a6f70d25497aac6766e9530691db318fea5c3fa915925e2d9f2fc29",
                Phone = "23049394",
                Role = Enum.GetName(typeof(UserRole), UserRole.Administrator)!,
            };


            context.Users.Add(user);
            context.SaveChanges();
        }

        //private void SetupAes256()
        //{
        //    var aes256Config = _config.GetSection("Aes256");
        //    var aes256Key = aes256Config.GetValue<string>("Key")!;
        //    var aes256IV = aes256Config.GetValue<string>("IV")!;
        //    Aes256Provider.Init(aes256Key, aes256IV);
        //}

        //private void SetupJwt()
        //{
        //    var jwtSettings = _config.GetSection("JwtSettings");
        //    var jwtKey = jwtSettings.GetValue<string>("Key")!;
        //    JwtManager.Init(jwtKey);
        //}

        //private void SetupHmac512()
        //{
        //    var hmac512Config = _config.GetSection("Hmac512");
        //    var hmac512Key = hmac512Config.GetValue<string>("Key")!;
        //    Hmac512Provider.Init(hmac512Key);
        //}
    }
}
