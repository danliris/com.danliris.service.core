using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/master/garment-units")]
	public class GarmentUnitController : BasicController<GarmentUnitService, Unit, UnitViewModel, CoreDbContext>
	{
		private new static readonly string ApiVersion = "1.0";
		private GarmentUnitService Service;
		public GarmentUnitController(GarmentUnitService service) : base(service, ApiVersion)
		{
			Service = service;
		}

        [HttpGet("GarmentandSample")]
        public IActionResult GetGarmentandSample(string Order = "{}", int Page = 1, int Size = 25, [Bind(Prefix = "Select[]")] List<string> Select = null, string Keyword = "", string Filter = "{}")
        {

            VerifyUser();

            try
            {

                Tuple<List<Unit>, int, Dictionary<string, string>, List<string>> Data = Service.GarmentandSample(Order,Page, Size, Select, Keyword, Filter);
                Dictionary<string, object> Result =
                  new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                  .Ok<Unit, UnitViewModel>(Data.Item1, Service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

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