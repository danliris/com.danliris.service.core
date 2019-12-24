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

namespace Com.DanLiris.Service.Core.Test.Controllers.DivisionTest
{
    [Collection("TestFixture Collection")]
    public class DivisionBasicTest
    {
        private const string URI = "v1/master/divisions";

        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        public DivisionBasicTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        protected DivisionDataUtil DataUtil
        {
            get { return (DivisionDataUtil)this.TestFixture.Service.GetService(typeof(DivisionDataUtil)); }
        }

        protected DivisionService Service
        {
            get { return (DivisionService)this.TestFixture.Service.GetService(typeof(DivisionService)); }
        }


        public DivisionViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new DivisionViewModel()
            {
                Name = string.Format("Division {0}", guid),
                Code = string.Format("Divisino {0}", guid),
                COACode = "COACode",
                Description = "Description",
                
            };
        }


        [Fact]
        public async Task Post_Failed_Internal_Server_Error()
        {

            DivisionViewModel VM = null;
            var response = await this.Client.PostAsync(URI, new StringContent(JsonConvert.SerializeObject(VM).ToString(), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Post_Failed_Bad_Request()
        {
            DivisionViewModel VM = GenerateTestModel();
            VM.Code = null;
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
        public async Task NotFound()
        {
            var response = await this.Client.GetAsync(string.Concat(URI, "/", 0));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
