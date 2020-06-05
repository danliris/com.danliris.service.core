using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
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

namespace Com.DanLiris.Service.Core.Test.Controllers.FabricTypeTest
{
    [Collection("TestFixture Collection")]
    public class ShippingGuyTest
    {
        private const string URI = "v1/master/fabrictypes";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ShippingGuyTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected FabricTypeServiceDataUtil DataUtil
        {
            get { return (FabricTypeServiceDataUtil)this.TestFixture.Service.GetService(typeof(FabricTypeServiceDataUtil)); }
        }

        protected FabricTypeService Service
        {
            get { return (FabricTypeService)this.TestFixture.Service.GetService(typeof(FabricTypeService)); }
        }

        public FabricTypeViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new FabricTypeViewModel()
            {
                Name = string.Format("TEST {0}", guid),
            };
        }

        [Fact]
        public async Task Post()
        {
            FabricTypeViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            FabricTypeViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            FabricTypeViewModel VM = GenerateTestModel();
            VM.Name = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        public async Task Delete()
        {
            FabricType fabrictype = await DataUtil.GetTestDataAsync();
            FabricTypeViewModel VM = Service.MapToViewModel(fabrictype);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            FabricType fabrictype = await DataUtil.GetTestDataAsync();
            FabricTypeViewModel VM = Service.MapToViewModel(fabrictype);
            var response = await this.Client.PutAsync(string.Concat(URI, "/", VM.Id), new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task NotFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSimple()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }     
    }
}
