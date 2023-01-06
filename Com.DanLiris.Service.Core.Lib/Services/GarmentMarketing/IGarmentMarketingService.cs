using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using System;
using System.Linq.Expressions;

namespace Com.DanLiris.Service.Core.Lib.Services.GarmentMarketing
{
    public interface IGarmentMarketingService : IBaseService<GarmentMarketingModel>
    {
        bool CheckExisting(Expression<Func<GarmentMarketingModel, bool>> filter);
    }
}
