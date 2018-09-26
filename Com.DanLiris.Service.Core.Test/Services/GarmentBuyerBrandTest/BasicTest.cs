using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentBuyerBrandTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class BasicTest : BasicServiceTest<CoreDbContext, GarmentBuyerBrandService, GarmentBuyerBrand>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Code" };
        public BasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(GarmentBuyerBrand model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;

        }

        public override void EmptyUpdateModel(GarmentBuyerBrand model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;

        }
        public override GarmentBuyerBrand GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentBuyerBrand()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
            };
        }
    }
}