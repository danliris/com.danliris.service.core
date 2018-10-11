using Com.DanLiris.Service.Core.Lib.ViewModels;
using System;
using Newtonsoft.Json;
using System.Net;
using Models = Com.DanLiris.Service.Core.Lib.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Test.DataUtils;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentCurrency
{
	[Collection("TestFixture Collection")]
	public class BasicTests : BasicControllerTest<CoreDbContext, GarmentCurrencyService, Models.GarmentCurrency, GarmentCurrencyViewModel, GarmentCurrencyDataUtil>
	{
		private const string URI = "v1/master/budget-currencies";
		private static List<string> CreateValidationAttributes = new List<string> { };
		private static List<string> UpdateValidationAttributes = new List<string> { };

		public BasicTests(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
		{
		}



	}
}
