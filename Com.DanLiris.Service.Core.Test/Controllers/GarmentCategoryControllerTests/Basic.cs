using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentCategoryControllerTests
{
    [Collection("TestFixture Collection")]
    public class Basic
    {
        private const string URI = "v1/master/garment-categories";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public Basic(TestServerFixture fixture)
        {
            TestFixture = fixture;

        }

        protected GarmentCategoryDataUtil DataUtil
        {
            get { return (GarmentCategoryDataUtil)this.TestFixture.Service.GetService(typeof(GarmentCategoryDataUtil)); }
        }

        public GarmentCategoryViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentCategoryViewModel()
            {
                code = guid,
                name = string.Format("TEST g-category {0}", guid),
                codeRequirement = string.Format("TEST g-category {0}", guid),
                categoryType = string.Format("TEST g-category {0}", guid),
                UOM = new GarmentCategoryUomViewModel
                {
                    Id = 1,
                    Unit = string.Format("TEST g-category {0}", guid)
                }
            };
        }

        [Fact]
        public async Task GetById()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byId"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByName()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byName"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByCodes()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/byCode"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post()
        {
            GarmentCategoryViewModel categoryViewModel = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(categoryViewModel).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

    }
}