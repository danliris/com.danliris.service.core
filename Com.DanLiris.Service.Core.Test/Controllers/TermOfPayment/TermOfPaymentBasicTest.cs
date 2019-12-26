using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.TermOfPaymentTest
{
    [Collection("TestFixture Collection")]
    public class TermOfPaymentBasicTest : BasicControllerTest<CoreDbContext, TermOfPaymentService, TermOfPayment, TermOfPaymentViewModel, TermOfPaymentDataUtil>
    {
        private const string URI = "v1/master/term-of-payments";

        private static List<string> CreateValidationAttributes = new List<string> { "Name", "Code" };
        private static List<string> UpdateValidationAttributes = new List<string> { "Name", "Code" };

        public TermOfPaymentBasicTest(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
        {

        }

    }
}
