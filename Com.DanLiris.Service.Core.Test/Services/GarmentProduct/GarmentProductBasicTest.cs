using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.StandardTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class GarmentProductBasicTest : BasicServiceTest<CoreDbContext, GarmentProductService, Models.GarmentProduct>
    {
        private static readonly string[] createAttrAssertions = { "Code" };
        private static readonly string[] updateAttrAssertions = { "Code" };
        private static readonly string[] existAttrCriteria = { "Code" };

        public GarmentProductBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.GarmentProduct model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Const = string.Empty;
            model.Yarn = string.Empty;
            model.Width = string.Empty;
            model.Tags = string.Empty;
            model.UomUnit = string.Empty;
            model.Remark = string.Empty;
            model.UomId = 0;
        }

        public override void EmptyUpdateModel(Models.GarmentProduct model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
            model.Const = string.Empty;
            model.Yarn = string.Empty;
            model.Width = string.Empty;
            model.Tags = string.Empty;
            model.UomUnit = string.Empty;
            model.Remark = string.Empty;
            model.UomId = 0;
        }

        public override Models.GarmentProduct GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.GarmentProduct()
            {
                Code = "Code",
                Name = string.Format("TEST {0}", guid),
                Active = true,
                Width = "1",
                Const = "const",
                Yarn = "yarn",
                Remark = "remark",
                Tags = "tags",
                UomId = 1,
                UomUnit = "uom",
            };
        }

        [SkippableFact]
        public override async Task TestCreateModel_Exist()
        {
            Skip.If(true);
        }
    }
}
