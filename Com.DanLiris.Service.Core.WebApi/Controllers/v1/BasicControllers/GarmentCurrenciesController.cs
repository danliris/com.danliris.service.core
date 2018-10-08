using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/garment-currencies")]
    public class GarmentCurrenciesController : BasicController<GarmentCurrencyService, GarmentCurrency, GarmentCurrencyViewModel, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";
		GarmentCurrencyService service;

        public GarmentCurrenciesController(GarmentCurrencyService service) : base(service, ApiVersion)
		{
			this.service = service;
		}

		[HttpGet("byId")]
		public IActionResult GetByIds([Bind(Prefix = "garmentCurrencyList[]")]List<int> garmentCurrencyList)
		{
			try
			{

				service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

				List<GarmentCurrency> Data = service.GetByIds(garmentCurrencyList);

				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
					.Ok(Data);

				return Ok(Result);
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}


	}
}
