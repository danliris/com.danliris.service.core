using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Helpers;
using Com.DanLiris.Service.Core.Test.Interface;
using System;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class DesignMotiveDataUtil : BasicDataUtil<CoreDbContext, DesignMotiveService, DesignMotive>, IEmptyData<DesignMotiveViewModel>
    {
        public DesignMotiveDataUtil(CoreDbContext dbContext, BuyerService Service) : base(dbContext, Service)
        {
        }

        public DesignMotiveViewModel GetEmptyData()
        {
            return new DesignMotiveViewModel();
        }

        public override DesignMotive GetNewData()
        {
            string guid = Guid.NewGuid().ToString();

            return new DesignMotive()
            {
                Name = string.Format("TEST {0}", guid),
                Code = string.Format("TEST {0}", guid),
                Id = 0,
            };
        }

        public override async Task<DesignMotive> GetTestDataAsync()
        {
            var data = GetNewData();
            await Service.CreateModel(data);
            return data;
        }
    }
}
