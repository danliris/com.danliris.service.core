﻿using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class GarmentProductServiceDataUtil : BasicDataUtil<CoreDbContext, GarmentProductService, GarmentProduct>, IEmptyData<GarmentProductViewModel>
    {

        public GarmentProductServiceDataUtil(CoreDbContext dbContext, GarmentProductService service) : base(dbContext, service)
        {
        }

        public GarmentProductViewModel GetEmptyData()
        {
            GarmentProductViewModel Data = new GarmentProductViewModel();

            Data.Name = "";
            Data.Code = "";
            Data.Remark = "desc";
            Data.Width = "0";
            Data.Const = "const";
            Data.Yarn = "yarn";
            Data.Tags = "tags";
            return Data;
        }

        public override GarmentProduct GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentProduct TestData = new GarmentProduct
            {
                Code = "Code",
                Name = string.Format("TEST {0}", guid),
                Active = true,
                Width = "1",
                Const = "const",
                Yarn = "yarn",
                Remark = "remark",
                Tags = "tags",
                UomId = 1,
                UomUnit = "uom",
                UId = guid
            };

            return TestData;
        }

        public override async Task<GarmentProduct> GetTestDataAsync()
        {
            GarmentProduct Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}