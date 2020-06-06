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

namespace Com.DanLiris.Service.Core.Test.Controllers.ShippingStaffTest
{
    [Collection("TestFixture Collection")]
    public class ShippingStaffTest
    {
        private const string URI = "v1/master/shippingstaffs";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public ShippingStaffTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected ShippingStaffServiceDataUtil DataUtil
        {
            get { return (ShippingStaffServiceDataUtil)this.TestFixture.Service.GetService(typeof(ShippingStaffServiceDataUtil)); }
        }

        protected ShippingStaffService Service
        {
            get { return (ShippingStaffService)this.TestFixture.Service.GetService(typeof(ShippingStaffService)); }
        }

        public ShippingStaffViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new ShippingStaffViewModel()
            {
                Name = string.Format("TEST {0}", guid),
            };
        }

        [Fact]
        public async Task Post()
        {
            ShippingStaffViewModel VM = GenerateTestModel();
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            ShippingStaffViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            ShippingStaffViewModel VM = GenerateTestModel();
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
            ShippingStaff shippingstaff = await DataUtil.GetTestDataAsync();
            ShippingStaffViewModel VM = Service.MapToViewModel(shippingstaff);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Update()
        {
            ShippingStaff shippingstaff = await DataUtil.GetTestDataAsync();
            ShippingStaffViewModel VM = Service.MapToViewModel(shippingstaff);
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
