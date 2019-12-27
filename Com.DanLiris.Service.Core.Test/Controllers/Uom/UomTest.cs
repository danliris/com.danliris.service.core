using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Core.Test.Controllers.UomTest
{
    [Collection("TestFixture Collection")]
    public class UomTest
    {
        private const string URI = "v1/master/uoms";
        protected TestServerFixture TestFixture { get; set; }

        protected HttpClient Client
        {
            get { return this.TestFixture.Client; }
        }

        protected UomService Service
        {
            get { return (UomService)this.TestFixture.Service.GetService(typeof(UomService)); }
        }

        protected UomServiceDataUtil DataUtil
        {
            get { return (UomServiceDataUtil)this.TestFixture.Service.GetService(typeof(UomServiceDataUtil)); }
        }

        public UomTest(TestServerFixture fixture)
        {
            TestFixture = fixture;
        }

        public UomViewModel GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new UomViewModel()
            {
                Unit = "uom",
            };
        }

        [Fact]
        public async Task GetSimple()
        {
            List<Uom> Data = Service.GetSimple();
            var result = Data.Select(x => Service.MapToViewModel(x));
            var response = await this.Client.GetAsync(string.Concat(URI, "/simple"));
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
            Uom uom = await DataUtil.GetTestDataAsync();
            UomViewModel VM = Service.MapToViewModel(uom);
            var response = await this.Client.DeleteAsync(string.Concat(URI, "/", VM.Id));

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
