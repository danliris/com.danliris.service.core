using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;
using Models = Com.DanLiris.Service.Core.Lib.Models;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentDetailCurrency
{
	[Collection("ServiceProviderFixture Collection")]
	public class GarmentDetailCurrencyBasicTest : BasicServiceTest<CoreDbContext, GarmentDetailCurrencyService, Models.GarmentDetailCurrency>
	{
		private static readonly string[] createAttrAssertions = { "Code","Date","Rate" };
		private static readonly string[] updateAttrAssertions = { "Code","Date","Rate" };
		private static readonly string[] existAttrCriteria = { "Code","Date" };

		private GarmentDetailCurrencyDataUtil DataUtil
		{
			get { return (GarmentDetailCurrencyDataUtil)ServiceProvider.GetService(typeof(GarmentDetailCurrencyDataUtil)); }
		}

		private GarmentDetailCurrencyService Services
		{
			get { return (GarmentDetailCurrencyService)ServiceProvider.GetService(typeof(GarmentDetailCurrencyService)); }
		}

		public GarmentDetailCurrencyBasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
		{
		}

		public override void EmptyCreateModel(Models.GarmentDetailCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override void EmptyUpdateModel(Models.GarmentDetailCurrency model)
		{
			model.Code = string.Empty;
			model.Date = DateTime.MaxValue;
			model.Rate = -1;
		}

		public override Models.GarmentDetailCurrency GenerateTestModel()
		{
			string guid = Guid.NewGuid().ToString();

			return new Models.GarmentDetailCurrency()
			{
				Code = guid,
				Date = DateTime.Today,
				Rate = 0,
			};
		}

		[Fact]
		public async void Should_Success_Get_Data_By_Id()
		{
			Models.GarmentDetailCurrency model1 = await DataUtil.GetTestDataAsync();
			Models.GarmentDetailCurrency model2 = await DataUtil.GetTestDataAsync();
			var Response = Services.GetByIds(new List<int> { model1.Id, model2.Id });
			Assert.NotNull(Response);
		}

        [Fact]
        public async void Should_Success_Get_Data_By_Code()
        {
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByCode(model.Code);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Single_Data_By_Code()
        {
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetSingleByCode(model.Code);
            Assert.NotNull(Response);
        }

        [Fact]
        public async void Should_Success_Get_Single_Data_By_Code_Date()
        {
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetSingleByCodeDate(model.Code, model.Date);
            Assert.NotNull(Response);
        }

 		[Fact]
		public async void Should_Success_ReadModel()
		{
			Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
			
			var orderData = new
			{
				Code="asc"
			};
			string order = JsonConvert.SerializeObject(orderData);

			var Response = Services.ReadModel(1, 25, order, new List<string>(), "", "{}");
			Assert.NotNull(Response);
		}

        [Fact]
        public async void Should_Success_Get_Single_Data_By_Code_Date_PEB()
        {
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            List<GarmentDetailCurrencyViewModel> garmentCurrencies = new List<GarmentDetailCurrencyViewModel>
            {
                new GarmentDetailCurrencyViewModel
                {
                    code = model.Code,
                    date = model.Date
                }
            };
            var Response = Services.GetSingleByCodeDatePEB(garmentCurrencies);
            Assert.NotNull(Response);
        }
    }
}
