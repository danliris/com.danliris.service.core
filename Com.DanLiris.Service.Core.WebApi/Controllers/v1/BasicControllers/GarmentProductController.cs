using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/garmentProducts")]
    public class GarmentProductsController : BasicController<GarmentProductService, GarmentProduct, GarmentProductViewModel, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";
        GarmentProductService service;

        public GarmentProductsController(GarmentProductService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpGet("byId")]
        public IActionResult GetByIds([Bind(Prefix = "garmentProductList[]")]List<int> garmentProductList)
        {
            try
            {
                
                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<GarmentProduct> Data = service.GetByIds(garmentProductList);

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
