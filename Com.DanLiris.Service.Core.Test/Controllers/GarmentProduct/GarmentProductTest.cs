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
    }
}
