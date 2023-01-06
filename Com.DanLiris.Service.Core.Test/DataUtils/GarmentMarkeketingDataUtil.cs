using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.GarmentMarketing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class GarmentMarketingDataUtil
    {
        private readonly GarmentMarketingService Service;

        public GarmentMarketingDataUtil(GarmentMarketingService service)
        {
            Service = service;
        }

        public GarmentMarketingModel GetNewData()
        {
            Guid guid = Guid.NewGuid();

            GarmentMarketingModel model = new GarmentMarketingModel
            {
                Name = $"Name{guid}",
                ResponsibleName = $"ResponsibleName{guid}",
            };

            return model;
        }

        public async Task<GarmentMarketingModel> GetTestData()
        {
            var data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }
    }
}
