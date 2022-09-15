using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.GarmentWareHouse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class GarmentWareHouseDataUtil
    {
        private readonly GarmentWareHouseService Service;

        public GarmentWareHouseDataUtil(GarmentWareHouseService service)
        {
            Service = service;
        }

        public GarmentWareHouseModel GetNewData()
        {
            Guid guid = Guid.NewGuid();

            GarmentWareHouseModel model = new GarmentWareHouseModel
            {
                Code = $"Code{guid}",
                Name = $"Name{guid}",
                Address = $"Address{guid}",
                Attention = $"Attention{guid}",
                PhoneNumber = $"PhoneNumber{guid}",
                FaxNumber = $"FaxNumber{guid}",
                Email = $"Email{guid}",
                NPWP = $"NPWP{guid}",
            };

            return model;
        }

        public async Task<GarmentWareHouseModel> GetTestData()
        {
            var data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }
    }
}
