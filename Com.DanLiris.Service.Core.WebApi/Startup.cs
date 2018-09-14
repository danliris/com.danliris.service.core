using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Com.DanLiris.Service.Core.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles;

namespace Com.DanLiris.Service.Core.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];
            string authority = Configuration["Authority"];
            string clientId = Configuration["ClientId"];
            string secret = Configuration["Secret"];

            services
                .AddDbContext<CoreDbContext>(options => options.UseSqlServer(connectionString))
                .AddScoped<AccountBankService>()
                //.AddScoped<AccountService>()
                .AddScoped<DesignMotiveService>()
                .AddScoped<BudgetService>()
                .AddScoped<BuyerService>()
                .AddScoped<CategoryService>()
                .AddScoped<CurrencyService>()
                .AddScoped<DivisionService>()
                .AddScoped<DesignMotiveService>()
                .AddScoped<GarmentCurrencyService>()
                .AddScoped<GarmentBuyerService>()
                .AddScoped<GarmentComodityService>()
                .AddScoped<HolidayService>()
                .AddScoped<ProductService>()
                .AddScoped<StorageService>()
                .AddScoped<SupplierService>()
                .AddScoped<TermOfPaymentService>()
                .AddScoped<UnitService>()
                .AddScoped<UomService>()
                .AddScoped<IncomeTaxService>()
                .AddScoped<QualityService>()
                .AddScoped<ComodityService>()
                .AddScoped<OrderTypeService>()
                .AddScoped<YarnMaterialService>()
                .AddScoped<MaterialConstructionService>()
                .AddScoped<ProcessTypeService>()
                .AddScoped<FinishTypeService>()
                .AddScoped<StandardTestsService>()
                .AddScoped<LampStandardService>()
                .AddScoped<PermissionService>()
                .AddScoped<ColorTypeService>()
                .AddScoped<RolesService>()
                .AddScoped<GarmentProductService>()
                .AddScoped<GarmentCategoryService>()
                .AddScoped<RolesService>();
            services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 1);
                });

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.ApiName = "com.danliris.service";
            //        options.ApiSecret = secret;
            //        options.Authority = authority;
            //        options.RequireHttpsMetadata = false;
            //        options.NameClaimType = JwtClaimTypes.Name;
            //    });

            string Secret = Configuration.GetValue<string>("Secret") ?? Configuration["Secret"];
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });

            services.AddCors(o => o.AddPolicy("CorePolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time");
            }));

            services
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CoreDbContext>();
                context.Database.Migrate();
            }
            app.UseAuthentication();
            app.UseCors("CorePolicy");

            app.UseMvc();
        }
    }
}
