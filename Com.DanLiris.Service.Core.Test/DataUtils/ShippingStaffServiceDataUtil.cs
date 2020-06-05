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
    public class ShippingStaffServiceDataUtil : BasicDataUtil<CoreDbContext, ShippingStaffService, ShippingStaff>, IEmptyData<ShippingStaffViewModel>
    {
        public ShippingStaffServiceDataUtil(CoreDbContext dbContext, ShippingStaffService service) : base(dbContext, service)
        {
        }

        public ShippingStaffViewModel GetEmptyData()
        {
            ShippingStaffViewModel Data = new ShippingStaffViewModel();

            Data.Name = "";
            return Data;
        }

        public override ShippingStaff GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            ShippingStaff TestData = new ShippingStaff
            {
                Active = true,
                Name = string.Format("TEST {0}", guid),
                UId = guid
            };

            return TestData;
        }

        public override async Task<ShippingStaff> GetTestDataAsync()
        {
            ShippingStaff Data = GetNewData();
            await this.Service.CreateModel(Data);
            return Data;
        }
    }
}
