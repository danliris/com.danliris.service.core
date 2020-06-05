using Com.DanLiris.Service.Core.Lib;
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
    public class FabricTypeServiceDataUtil : BasicDataUtil<CoreDbContext, FabricTypeService, FabricType>, IEmptyData<FabricTypeViewModel>
    {
        public FabricTypeServiceDataUtil(CoreDbContext dbContext, FabricTypeService service) : base(dbContext, service)
        {
        }

        public FabricTypeViewModel GetEmptyData()
        {
            FabricTypeViewModel Data = new FabricTypeViewModel();

            Data.Name = "";
            return Data;
        }

        public override FabricType GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            FabricType TestData = new FabricType
            {
                Active = true,
                Name = string.Format("TEST {0}", guid),
                UId = guid
            };

            return TestData;
        }

        public override async Task<FabricType> GetTestDataAsync()
        {
            FabricType Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}
