using AutoMapper;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Com.DanLiris.Service.Core.Test.Controllers.Product
{
    [Collection("TestFixture Collection")]
    public class ProductTest
    {
        private const string URI = "v1/master/products";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ProductTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public ProductViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ProductViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = "Code",
                Active = true,
                Description = "desc",
                Price = 12,
                Tags = "tags",
                UOM = new ProductUomViewModel { Unit = "unit", Id = 1 },
                Currency = new ProductCurrencyViewModel { Symbol = "rp", Code = "idr", Id = 1 },
            };
        }

        protected ProductServiceDataUtil DataUtil
        {
            get { return (ProductServiceDataUtil)this.TestFixture.Service.GetService(typeof(ProductServiceDataUtil)); }
        }

        public string GeneratePackingModel()
        {
            string content = "{\"PackingDetails\":[{\"Weight\":3,\"Quantity\":2,\"Length\":4,\"Lot\":\"2\",\"Grade\":\"BS FINISH\",\"Remark\":\"1\"},{\"Weight\":11,\"Quantity\":22,\"Length\":11,\"Lot\":\"321\",\"Grade\":\"A\"}],\"DeliveryType\":\"BARU\",\"FinishedProductType\":\"WHITE\",\"OrderTypeName\":\"YARN DYED\",\"ColorName\":\"black\",\"Material\":\"MATERIAL 02\",\"MaterialWidthFinish\":\"3\",\"Date\":\"2018 - 09 - 03T17: 00:00.000Z\",\"PackingUom\":\"ROLL\",\"BuyerId\":25,\"BuyerCode\":\"A000A\",\"BuyerName\":\"ALI IMRON\",\"BuyerAddress\":\"S O L O\",\"BuyerType\":\"Lokal\",\"ProductionOrderId\":43,\"ProductionOrderNo\":\"F / 2018 / 0001\",\"OrderTypeId\":6,\"OrderTypeCode\":\"GH9YFUL5\",\"SalesContractNo\":\"0001 / FPL / 9 / 2018\",\"DesignNumber\":null,\"DesignCode\":null,\"ColorType\":null,\"MaterialId\":2,\"MaterialConstructionFinishId\":2,\"MaterialConstructionFinishName\":\"118x84\"}";
            return content;
        }

        [Fact]
        public async Task Get()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNulTags()
        {
            var response = await this.Client.GetAsync(URI + "/null-tags");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Exception_GetNulTags()
        {
            var response = await this.Client.GetAsync(URI + "/null-tags?Select=null&Keyword=null&Filter=name");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByIdForSpinning()
        {
            var Model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync(string.Concat(URI, "/spinning/", Model.Id));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByIdForSpinning_NoFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/spinning/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post()
        {

            ProductViewModel productViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(productViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostPacking()
        {
            string content = GeneratePackingModel();
            var response = await this.Client.PostAsync(URI + "/packing/create", new StringContent(content, Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetByProductionOrderNo()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byProductionOrderNo"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetSimple()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byId"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_GetByName()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/by-name"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private ProductsController GetController(IServiceProvider serviceProvider)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            var controller = (ProductsController)Activator.CreateInstance(typeof(ProductsController), serviceProvider);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        private int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private CoreDbContext GetDbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();

            DbContextOptionsBuilder<CoreDbContext> optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            CoreDbContext dbContext = new CoreDbContext(optionsBuilder.Options);

            return dbContext;
        }
        [Fact]
        public void Should_Fail_Get_Xls_Data()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IIdentityService))).Returns(new IdentityService());

            var serviceMock = new Mock<ProductService>();
            serviceMock.Setup(f => f.ReadModel(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ProductService))).Returns(serviceMock.Object);

            var controller = GetController(serviceProviderMock.Object);
            var response = controller.Get();

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}