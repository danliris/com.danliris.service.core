﻿using Com.Danliris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Com.DanLiris.Service.Core.Test.DataUtils;
using Com.DanLiris.Service.Core.Lib.ViewModels;

namespace Com.DanLiris.Service.Core.Test.Services.GarmentBuyerBrandTest
{
    [Collection("ServiceProviderFixture Collection")]
    public class BasicTest : BasicServiceTest<CoreDbContext, GarmentBuyerBrandService, GarmentBuyerBrand>
    {
      private static readonly string[] createAttrAssertions = { "Name", "BuyerCode","Code","BuyerName" };
        private static readonly string[] updateAttrAssertions = { "Name" ,"BuyerCode"};
        private static readonly string[] existAttrCriteria = { "Code" ,"Name"};
        public BasicTest(ServiceProviderFixture fixture) : base(fixture, createAttrAssertions, updateAttrAssertions, existAttrCriteria)
        {
        }
        public override void EmptyCreateModel(GarmentBuyerBrand model)
        {
           
            model.BuyerName = string.Empty;

        }
        private GarmentBuyerBrandDataUtil DataUtil
        {
            get { return (GarmentBuyerBrandDataUtil)ServiceProvider.GetService(typeof(GarmentBuyerBrandDataUtil)); }
        }
        private GarmentBuyerBrandService Services
        {
            get { return (GarmentBuyerBrandService)ServiceProvider.GetService(typeof(GarmentBuyerBrandService)); }
        }

        public override void EmptyUpdateModel(GarmentBuyerBrand model)
        {
           
            model.BuyerName = string.Empty;

        }
        public override GarmentBuyerBrand GenerateTestModel()
        {
            string guid = Guid.NewGuid().ToString();

            return new GarmentBuyerBrand()
            {
                Code = guid,
                Name = string.Format("TEST {0}", guid),
                BuyerCode = "Buyer" + guid 
            };
        }
        [Fact]
        public async void GetByName()
        {
            GarmentBuyerBrand model = await DataUtil.GetTestDataAsync();
            var Response = Services.GetByName(model.Name, "{\"Name\":\"Name\"}");
            Assert.NotNull(Response);
        }
        [Fact]
        public async void Upload()
        {
            
            GarmentBuyerBrandViewModel model =  DataUtil.GetUploadData();
            List<GarmentBuyerBrandViewModel> viewModel = new List<GarmentBuyerBrandViewModel>();
            viewModel.Add(model); 
            var Response = Services.UploadValidate(viewModel,null);
            Assert.Equal(Response.Item1, false);
        }
      

    }
}