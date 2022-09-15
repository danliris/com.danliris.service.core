using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Linq.Expressions;

namespace Com.DanLiris.Service.Core.Lib.Services.GarmentWareHouse
{
    public interface IGarmentWareHouseService : IBaseService<GarmentWareHouseModel>
    {
        bool CheckExisting(Expression<Func<GarmentWareHouseModel, bool>> filter);
    }
}
