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
    public class ShippingStaffBasicTest : BasicServiceTest<CoreDbContext, ShippingStaffService, Models.ShippingStaff>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public ShippingStaffBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.ShippingStaff model)
        {
            model.Name = string.Empty;
        }
        public override void EmptyUpdateModel(Models.ShippingStaff model)
        {
            model.Name = string.Empty;
        }
        public override Models.ShippingStaff GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.ShippingStaff()
            {
                Name = string.Format("TEST {0}", guid),
            };
        }
        
        //[Fact]
        //public void TestSimple()
        //{
        //    var data = Service.GetSimple();
        //    Assert.NotNull(data);
        //}
        
    }
}
