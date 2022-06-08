using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class GarmentCategoryDataUtil : BasicDataUtil<CoreDbContext, GarmentCategoryService, GarmentCategory>, IEmptyData<GarmentCategoryViewModel>
    {

        public GarmentCategoryDataUtil(CoreDbContext dbContext, GarmentCategoryService service) : base(dbContext, service)
        {
        }

        public GarmentCategoryViewModel GetEmptyData()
        {
            GarmentCategoryViewModel Data = new GarmentCategoryViewModel();

            Data.name = "";
            Data.codeRequirement = "";
            Data.categoryType = "";
            Data.code = "";
            Data.UOM = null;
            return Data;
        }

        public GarmentCategoryViewModel GetNewData_GarmentCategoryViewModel()
        {
            GarmentCategoryViewModel Data = new GarmentCategoryViewModel();

            Data.name = "";
            Data.codeRequirement = "";
            Data.categoryType = "";
            Data.code = "";
            Data.UOM = new GarmentCategoryUomViewModel()
            {
                Unit = ""
            };
            return Data;
        }

        public override GarmentCategory GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentCategory TestData = new GarmentCategory
            {
                Name = string.Format("TEST {0}", guid),
                CodeRequirement = string.Format("TEST {0}", guid),
                CategoryType = string.Format("TEST {0}", guid),
                UomId = 1,
                UomUnit = "uom",
                Code = string.Format("TEST {0}", guid),
                UId = guid
            };

            return TestData;
        }

        public GarmentCategory GetNewData2()
        {
            string guid = Guid.NewGuid().ToString();
            GarmentCategory TestData = new GarmentCategory
            {
                Name = string.Format("TEST {0}", guid),
                CodeRequirement = string.Format("TEST {0}", guid),
                CategoryType = string.Format("TEST {0}", guid),
                UomId = 1,
                UomUnit = "uom",
                Code = string.Format("TEST {0}", guid),
                UId = guid
            };

            return TestData;
        }

        public override async Task<GarmentCategory> GetTestDataAsync()
        {
            GarmentCategory Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}