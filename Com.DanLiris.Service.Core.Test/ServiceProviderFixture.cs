using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Com.DanLiris.Service.Core.Test
{
    public class ServiceProviderFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public ServiceProviderFixture()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Secret", "DANLIRISTESTENVIRONMENT"),
					new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
					new KeyValuePair<string, string>("DefaultConnection",  "Server=localhost,1401;Database=com.danliris.db.core.controller.test;User=sa;password=Standar123.;MultipleActiveResultSets=true;")

                })
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["DefaultConnection"];

            this.ServiceProvider = new ServiceCollection()
                .AddDbContext<CoreDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Transient)
                .AddTransient<BudgetService>(provider => new BudgetService(provider))
                .AddTransient<BudgetServiceDataUtil>()
                .AddTransient<ComodityService>(provider => new ComodityService(provider))
                .AddTransient<ComodityServiceDataUtil>()
                .AddTransient<QualityService>(provider => new QualityService(provider))
                .AddTransient<QualityServiceDataUtil>()
                .AddTransient<YarnMaterialService>(provider => new YarnMaterialService(provider))
                .AddTransient<YarnMaterialServiceDataUtil>()
                .AddTransient<MaterialConstructionService>(provider => new MaterialConstructionService(provider))
                .AddTransient<MaterialConstructionServiceDataUtil>()
                .AddTransient<IncomeTaxService>(provider => new IncomeTaxService(provider))
                .AddTransient<IncomeTaxDataUtil>()
                .AddTransient<LampStandardService>(provider => new LampStandardService(provider))
                .AddTransient<LampStandardDataUtil>()
                .AddTransient<StandardTestsService>(provider => new StandardTestsService(provider))
                .AddTransient<StandardTestDataUtil>()
                .AddTransient<DivisionService>(provider => new DivisionService(provider))
                .AddTransient<ProductService>(provider => new ProductService(provider))
                .AddTransient<ProductServiceDataUtil>()
                .AddTransient<AccountBankDataUtil>()
                .AddTransient<AccountBankService>(provider => new AccountBankService(provider))
                .AddTransient<GarmentProductService>(provider => new GarmentProductService(provider))
                .AddTransient<GarmentProductServiceDataUtil>()
                .AddTransient<GarmentCategoryDataUtil>()
                .AddTransient<GarmentCategoryService>(provider => new GarmentCategoryService(provider))
                .AddTransient<GarmentBuyerService>(provider => new GarmentBuyerService(provider))
                .AddTransient<GarmentComodityService>(provider => new GarmentComodityService(provider))
				.AddTransient<GarmentSupplierDataUtil>()
				.AddTransient<GarmentSupplierService>(provider => new GarmentSupplierService(provider))
				.AddTransient<GarmentUnitService>(provider => new GarmentUnitService(provider))
                .AddTransient<GarmentBuyerBrandService>(provider => new GarmentBuyerBrandService(provider))
                .AddTransient<UnitService>(provider => new UnitService(provider))
				.BuildServiceProvider();

            CoreDbContext dbContext = ServiceProvider.GetService<CoreDbContext>();
            dbContext.Database.Migrate();
        }

        public void Dispose()
        {
        }
    }

    [CollectionDefinition("ServiceProviderFixture Collection")]
    public class ServiceProviderFixtureCollection : ICollectionFixture<ServiceProviderFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}