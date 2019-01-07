using AutoMapper;
using Com.Danliris.Service.Core.Data.Migration.MigrationServices;
using Com.Danliris.Service.Core.Mongo;
using Com.Danliris.Service.Core.Mongo.MongoRepositories;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Helpers.ValidateService;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Services.Account_and_Roles;
using Com.DanLiris.Service.Core.Lib.Services.MachineSpinning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Com.DanLiris.Service.Core.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<IIdentityService, IdentityService>()
                .AddScoped<IValidateService, ValidateService>();
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
                .AddScoped<BudgetCurrencyService>()
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
                .AddScoped<GarmentSupplierService>()
                .AddScoped<GarmentUnitService>()
                .AddScoped<GarmentBuyerBrandService>()
                .AddTransient<IMachineSpinningService, MachineSpinningService>()
                .AddScoped<RolesService>();

            services.Configure<MongoDbSettings>(
                options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"];
                    options.Database = Configuration.GetConnectionString("MongoDatabase") ?? Configuration["MongoDatabase"];
                });

            services.AddSingleton<IMongoClient, MongoClient>(
                _ => new MongoClient(Configuration.GetConnectionString("MongoConnection") ?? Configuration["MongoConnection"]));

            services.AddTransient<IMongoDbContext, MongoDbContext>();
            services.AddTransient<IUnitOfMeasurementMongoRepository, UnitOfMeasurementMongoRepository>();
            services.AddTransient<IUnitOfMeasurementMigrationService, UnitOfMeasurementMigrationService>();
            services.AddTransient<ICurrencyMongoRepository, CurrencyMongoRepository>();
            services.AddTransient<ICurrencyMigrationService, CurrencyMigrationService>();
            services.AddTransient<IProductMongoRepository, ProductMongoRepository>();
            services.AddTransient<IProductMigrationService, ProductMigrationService>();
            services.AddTransient<ISupplierMongoRepository, SupplierMongoRepository>();
            services.AddTransient<ISupplierMigrationService, SupplierMigrationService>();

            RegisterServices(services);

            services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 1);
                });

            services.AddAutoMapper();

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
