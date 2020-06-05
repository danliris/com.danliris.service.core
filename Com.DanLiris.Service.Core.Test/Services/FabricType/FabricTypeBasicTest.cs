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
    public class FabricTypeBasicTest : BasicServiceTest<CoreDbContext, FabricTypeService, Models.FabricType>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public FabricTypeBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(Models.FabricType model)
        {
            model.Name = string.Empty;
        }
        public override void EmptyUpdateModel(Models.FabricType model)
        {
            model.Name = string.Empty;
        }
        public override Models.FabricType GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Models.FabricType()
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
