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
	[Route("v{version:apiVersion}/master/garment-suppliers")]
	public class GarmentSuppliersController : BasicController<GarmentSupplierService, GarmentSupplier, GarmentSupplierViewModel, CoreDbContext>
	{
		private new static readonly string ApiVersion = "1.0";
        GarmentSupplierService service;

        public GarmentSuppliersController(GarmentSupplierService service) : base(service, ApiVersion)
		{
            this.service = service;
        }


        [HttpGet("byCodes")]
        public IActionResult GetByIds([FromBody]string code)
        {
            try
            {

                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                List<GarmentSupplier> Data = service.GetByCodes(code);

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

        [HttpPut("posting")]
        public async Task<IActionResult> Posting([FromBody] List<GarmentSupplierViewModel> garmentsupplier)
        {
            try
            {
                service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await service.GarmentSupplierPost(garmentsupplier, service.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(garmentsupplier);

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

                await service.garmentsupplierNonActive(id, service.Username);


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
        //
        [HttpGet("download")]
        public IActionResult GetXls(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                //int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];
                byte[] xlsInBytes;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = service.GenerateExcel(dateFrom, dateTo);

                string filename = String.Format("Garment Supplier - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

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