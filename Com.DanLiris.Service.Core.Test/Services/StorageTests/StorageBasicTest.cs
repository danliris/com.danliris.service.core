using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.StorageTests
{
    [Collection("ServiceProviderFixture Collection")]
    public class StorageBasicTest : BasicServiceTest<CoreDbContext, StorageService, Storage>
    {
        private static readonly string[] createAttrAssertions = { "name" };
        private static readonly string[] updateAttrAssertions = { "name" };
        private static readonly string[] existAttrCriteria = null;

        public StorageBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(Storage model)
        {
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(Storage model)
        {
            model.Name = string.Empty;
        }

        public override Storage GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new Storage
            {
                Name = string.Format("StorageName {0}", guid),
                UnitId = 1,
                UnitName = "UnitName"
            };
        }
    }
}
