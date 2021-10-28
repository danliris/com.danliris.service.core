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
	public class GarmentDetailCurrencyDataUtil : BasicDataUtil<CoreDbContext, GarmentDetailCurrencyService, GarmentDetailCurrency>, IEmptyData<GarmentDetailCurrencyViewModel>
	{
		public GarmentDetailCurrencyDataUtil(CoreDbContext dbContext, GarmentDetailCurrencyService service) : base(dbContext, service)
		{
		}

		public GarmentDetailCurrencyViewModel GetEmptyData()
		{
			GarmentDetailCurrencyViewModel Data = new GarmentDetailCurrencyViewModel();
			
			return Data;
		}

		public override GarmentDetailCurrency GetNewData()
		{
            GarmentDetailCurrency model = new GarmentDetailCurrency();

			string guid = Guid.NewGuid().ToString();

			model.Code = guid;
			model.Date = DateTime.Now;
			model.Rate = 1;

			return model;
		}

		public override async Task<GarmentDetailCurrency> GetTestDataAsync()
		{
            GarmentDetailCurrency model = GetNewData();
			await this.Service.CreateModel(model);
			return model;
		}

		

		
	}
}
