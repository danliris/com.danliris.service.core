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
    public class StorageDataUtil : BasicDataUtil<CoreDbContext, StorageService, Storage>, IEmptyData<StorageViewModel>
    {
        public StorageDataUtil(CoreDbContext dbContext, StorageService service) : base(dbContext, service)
        {
        }

        public StorageViewModel GetEmptyData()
        {
            return new StorageViewModel();
        }

        public override Storage GetNewData()
        {
            string guid = Guid.NewGuid().ToString();
            return new Storage
            {
                Name = string.Format("StorageName {0}", guid),
                UnitId = 1,
            };
        }

        public override async Task<Storage> GetTestDataAsync()
        {
            var data = GetNewData();
            await Service.CreateModel(data);
            return data;
        }
    }
}
