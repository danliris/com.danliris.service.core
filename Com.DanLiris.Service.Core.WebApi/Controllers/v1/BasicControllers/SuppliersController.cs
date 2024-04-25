using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/suppliers")]
    public class SuppliersController : BasicController<SupplierService, Supplier, SupplierViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        SupplierService service;

        public SuppliersController(SupplierService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpPut("posting")]
        public async Task<IActionResult> Posting([FromBody] List<SupplierViewModel> supplier)
        {
            try
            {
                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await service.SupplierPost(supplier, service.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(supplier);

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

        [HttpPut("nonactived/{id}")]
        public async Task<IActionResult> NonActived([FromRoute] int id)
        {
            try
            {
                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await service.supplierNonActive(id, service.Username);


                Dictionary<string, object> Result =
                new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                .Ok();
                return NoContent();
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
