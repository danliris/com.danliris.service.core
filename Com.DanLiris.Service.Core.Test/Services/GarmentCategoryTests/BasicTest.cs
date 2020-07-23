using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Test.DataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentCategoryTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class BasicTest : BasicServiceTest<CoreDbContext, GarmentCategoryService, GarmentCategory>
    {
        private static readonly string[] createAttrAssertions = { "Name","Code","UomId","CodeRequirement", "CategoryType" };
        private static readonly string[] updateAttrAssertions = { "Name", "Code", "UomId", "CodeRequirement", "CategoryType" };
        private static readonly string[] existAttrCriteria = { "Name" };

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
                CodeRequirement= String.Concat("TEST G-Category ", guid),
                CategoryType = String.Concat("TEST G-Category ", guid),
                UomUnit = String.Concat("TEST G-Category ", guid),
                UomId=1
            };
        }

        [Fact]
        public void Should_Success_Get_Data()
        {
            GarmentCategory model = DataUtil.GetNewData2();
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
    }
}
