using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.DivisionTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class DivisionBasicTest : BasicServiceTest<CoreDbContext, DivisionService, Division>
    {
        private static readonly string[] createAttrAssertions = { "Name", "Code", "COACode", "Description", "UId" };
        private static readonly string[] updateAttrAssertions = { "Name", "Code", "COACode", "Description", "UId" };
        private static readonly string[] existAttrCriteria = { "Code" };

        public DivisionBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Division model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Division model)
        {
            model.Code = string.Empty;
            model.Name = string.Empty;
        }

        public override Division GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Division()
            {
                Name = string.Format("Division {0}", guid),
                Code = string.Format("Division {0}", guid),
                
            };
        }
    }
}
