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
using System.Text;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentDetailCurrency
{
	[Collection("TestFixture Collection")]
	public class BasicTests : BasicControllerTest<CoreDbContext, GarmentDetailCurrencyService, Models.GarmentDetailCurrency, GarmentDetailCurrencyViewModel, GarmentDetailCurrencyDataUtil>
	{
		private const string URI = "v1/master/garment-detail-currencies";
		private static List<string> CreateValidationAttributes = new List<string> { };
		private static List<string> UpdateValidationAttributes = new List<string> { };

		public BasicTests(TestServerFixture fixture) : base(fixture, URI, CreateValidationAttributes, UpdateValidationAttributes)
		{
		}

        protected new GarmentDetailCurrencyDataUtil DataUtil
        {
            get { return (GarmentDetailCurrencyDataUtil)this.TestFixture.Service.GetService(typeof(GarmentDetailCurrencyDataUtil)); }
        }

        [Fact]
        public async Task Should_Success_Get_ById()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/byId";
            var response = await this.Client.GetAsync(byCodeUri);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task Should_Success_Get_Data_By_Code()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/byCode";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}/{model.Code}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

       
        [Fact]
        public async Task Should_Success_Get_Single_Data_By_Code()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}/{model.Code}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Error_Get_Single_Data_By_Code()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}/any");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Get_Single_Data_By_Code_Date()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code-date";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}?code={model.Code}&stringDate={model.Date.ToString()}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Error_Get_Single_Data_By_Code_Date()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code-date";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}?stringDate={model.Date.ToString()}");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_Get_Single_Data_By_PEBDate()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/sales-debtor-currencies-peb";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();
            var response = await this.Client.GetAsync($"{byCodeUri}?stringDate={model.Date.ToString()}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Success_GetByCodeBeforeDatePEB()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code-date-peb";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();

            List<GarmentDetailCurrencyViewModel> garmentCurrencies = new List<GarmentDetailCurrencyViewModel>
            {
                new GarmentDetailCurrencyViewModel
                {
                    code = model.Code,
                    date = model.Date
                }
            };
            var request = new HttpRequestMessage(HttpMethod.Get, byCodeUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(garmentCurrencies), Encoding.Unicode, "application/json")
            };
            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Error_GetByCodeBeforeDatePEB()
        {
            string byCodeUri = "v1/master/garment-detail-currencies/single-by-code-date-peb";
            Models.GarmentDetailCurrency model = await DataUtil.GetTestDataAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, byCodeUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null), Encoding.Unicode, "application/json")
            };
            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

    }
}
