using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Test.DataUtils;
using System;
using System.Collections.Generic;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.BudgetCurrency
{
	[Collection("ServiceProviderFixture Collection")]
	public class BudgetCurrencyBasicTest : BasicServiceTest<CoreDbContext, BudgetCurrencyService, Models.BudgetCurrency>
	{
		private static readonly string[] createAttrAssertions = { "Code", "Date", "Rate" };
		private static readonly string[] updateAttrAssertions = { "Code", "Date", "Rate" };
		private static readonly string[] existAttrCriteria = { "Code", "Date" };
		
		private BudgetCurrencyDataUtil DataUtil
		{
			get { return (BudgetCurrencyDataUtil)ServiceProvider.GetService(typeof(BudgetCurrencyDataUtil)); }
		}

		private BudgetCurrencyService Services
		{
			get { return (BudgetCurrencyService)ServiceProvider.GetService(typeof(BudgetCurrencyService)); }
		}

		public BudgetCurrencyBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
		{
		}

		public override void EmptyCreateModel(Models.BudgetCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override void EmptyUpdateModel(Models.BudgetCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override Models.BudgetCurrency GenerateTestModel()
		{
			string guid = Guid.NewGuid().ToString();

			return new Models.BudgetCurrency()
			{
				Code = guid,
				Date = DateTime.Today,
				Rate = 0,
				Remark = guid
			};
		}

		[Fact]
		public async void Should_Success_Get_Data_By_Id()
		{
			Models.BudgetCurrency model1 = await DataUtil.GetTestDataAsync();
			Models.BudgetCurrency model2 = await DataUtil.GetTestDataAsync();
			var Response = Services.GetByIds(new List<int> { model1.Id, model2.Id });
			Assert.NotNull(Response);
		}

	}
}
