using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/garment-comodities")]
    public class GarmentComodityController : BasicController<GarmentComodityService, GarmentComodity, GarmentComodityViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        private GarmentComodityService Service;
        public GarmentComodityController(GarmentComodityService service) : base(service, ApiVersion)
        {
            Service = service;
        }

        [HttpGet("all")]
        public IActionResult Get()
        {

            VerifyUser();

            try
            {
                var data = Service.GetAllComodity();
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    TotalData = data.Item2,
                    data = data.Item1
                });
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
