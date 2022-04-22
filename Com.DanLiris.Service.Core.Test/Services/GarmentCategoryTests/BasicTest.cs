using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.Moonlay.NetCore.Lib.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentCategoryTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class BasicTest : BasicServiceTest<CoreDbContext, GarmentCategoryService, GarmentCategory>
    {
        private static readonly string[] createAttrAssertions = { "Name", "Code", "UomId", "CodeRequirement", "CategoryType" };
        private static readonly string[] updateAttrAssertions = { "Name", "Code", "UomId", "CodeRequirement", "CategoryType" };
        private static readonly string[] existAttrCriteria = { "Name", "Code" };

        public BasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        private GarmentCategoryDataUtil DataUtil
        {
            get { return (GarmentCategoryDataUtil)ServiceProvider.GetService(typeof(GarmentCategoryDataUtil)); }
        }

        private GarmentCategoryService Services
        {
            get { return (GarmentCategoryService)ServiceProvider.GetService(typeof(GarmentCategoryService)); }
        }

        public override void EmptyCreateModel(GarmentCategory model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.UomId = null;
            model.CodeRequirement = string.Empty;
            model.CategoryType = string.Empty;
        }

        public override void EmptyUpdateModel(GarmentCategory model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.UomId = null;
            model.CodeRequirement = string.Empty;
            model.CategoryType = string.Empty;
        }

        public override GarmentCategory GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentCategory()
            {
                Name = String.Concat("TEST G-Category ", guid),
                Code = guid,
                CodeRequirement = String.Concat("TEST G-Category ", guid),
                CategoryType = String.Concat("TEST G-Category ", guid),
                UomUnit = String.Concat("TEST G-Category ", guid),
                UomId = 1
            };
        }

        [Fact]
        public void Should_Success_MapToViewModel()
        {
            GarmentCategory model = DataUtil.GetNewData();
            var Response = Services.MapToViewModel(model);
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_MapToModel()
        {
            GarmentCategoryViewModel model = DataUtil.GetNewData_GarmentCategoryViewModel();
            var Response = Services.MapToModel(model);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_ReadModel()
        {
            GarmentCategory model = await DataUtil.GetTestDataAsync();

            var orderData = new
            {
                Code = "asc"
            };
            string order = JsonConvert.SerializeObject(orderData);

            var Response = Services.ReadModel(1, 25, order, new List<string>(), "", "{}");
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_Get_Data()
        {
            GarmentCategory model = DataUtil.GetNewData();
            var Response = Services.ReadModel(1, 25, "{\"Code\":\"desc\"}", null, "", "{}");
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            GarmentCategory model1 = await DataUtil.GetTestDataAsync();
            GarmentCategory model2 = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByIds(new List<int> { model1.Id, model2.Id });
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            GarmentCategory model1 = await DataUtil.GetTestDataAsync();
            GarmentCategory model2 = DataUtil.GetNewData2();
            var Response = Services.Create(model2);
            Assert.NotEqual(Response, 0);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Name()
        {
            GarmentCategory model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByName(model.Name);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Code()
        {
            GarmentCategory model1 = await DataUtil.GetTestDataAsync();
            GarmentCategory model2 = await DataUtil.GetTestDataAsync();

            var codes = string.Format("{0},{1}", model1.Code, model2.Code);
            var Response = Services.GetByCode(new List<string> { model1.Code, model2.Code });
            Assert.NotNull(Response);
        }

    }
}