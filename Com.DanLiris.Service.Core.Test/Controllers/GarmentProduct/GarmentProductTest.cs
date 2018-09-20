using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentProduct
{
    [Collection("TestFixture Collection")]
    public class GarmentProductTest
    {
        private const string URI = "v1/master/garmentProducts";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public GarmentProductTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public GarmentProductViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentProductViewModel()
            {
                Name = string.Format("TEST {0}", guid),
                Code = "Code",
                Active = true,
                Remark = "desc",
                Width = "12",
                Const = "const",
                Yarn = "yarn",
                Tags = "tags",
                ProductType = "FABRIC",
                Composition = "Composition",
                UOM = new GarmentProductUomViewModel { Unit = "unit", Id = 1 },
            };
        }

        //public string GeneratePackingModel()
        //{
        //    string content = "{\"PackingDetails\":[{\"Weight\":3,\"Quantity\":2,\"Length\":4,\"Lot\":\"2\",\"Grade\":\"BS FINISH\",\"Remark\":\"1\"},{\"Weight\":11,\"Quantity\":22,\"Length\":11,\"Lot\":\"321\",\"Grade\":\"A\"}],\"DeliveryType\":\"BARU\",\"FinishedProductType\":\"WHITE\",\"OrderTypeName\":\"YARN DYED\",\"ColorName\":\"black\",\"Material\":\"MATERIAL 02\",\"MaterialWidthFinish\":\"3\",\"Date\":\"2018 - 09 - 03T17: 00:00.000Z\",\"PackingUom\":\"ROLL\",\"BuyerId\":25,\"BuyerCode\":\"A000A\",\"BuyerName\":\"ALI IMRON\",\"BuyerAddress\":\"S O L O\",\"BuyerType\":\"Lokal\",\"ProductionOrderId\":43,\"ProductionOrderNo\":\"F / 2018 / 0001\",\"OrderTypeId\":6,\"OrderTypeCode\":\"GH9YFUL5\",\"SalesContractNo\":\"0001 / FPL / 9 / 2018\",\"DesignNumber\":null,\"DesignCode\":null,\"ColorType\":null,\"MaterialId\":2,\"MaterialConstructionFinishId\":2,\"MaterialConstructionFinishName\":\"118x84\"}";
        //    return content;
        //}

        [Fact]
        public async Task Get()
        {
            var response = await this.Client.GetAsync(URI);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post()
        {

            GarmentProductViewModel garmentProductViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(garmentProductViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        //[Fact]
        //public async Task PostPacking()
        //{
        //    string content = GeneratePackingModel();
        //    var response = await this.Client.PostAsync(URI + "/packing/create", new StringContent(content, Encoding.UTF8, "application/json"));

        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        //}

    }
}
