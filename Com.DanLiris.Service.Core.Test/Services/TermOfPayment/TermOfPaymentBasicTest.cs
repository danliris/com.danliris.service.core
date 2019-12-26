using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using System;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Services.TermOfPaymentTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class TermOfPaymentBasicTest : BasicServiceTest<CoreDbContext, TermOfPaymentService, TermOfPayment>
    {
        private static readonly string[] createAttrAssertions = { "Name" };
        private static readonly string[] updateAttrAssertions = { "Name" };
        private static readonly string[] existAttrCriteria = { "Name" };

        public TermOfPaymentBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }

        public override void EmptyCreateModel(TermOfPayment model)
        {
            model.Name = string.Empty;
        }

        public override void EmptyUpdateModel(TermOfPayment model)
        {
            model.Name = string.Empty;
        }

        public override TermOfPayment GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new TermOfPayment()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
                IsExport = false,
            };
        }
    }
}
