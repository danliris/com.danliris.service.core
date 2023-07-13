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
    [Route("v{version:apiVersion}/master/track")]
    public class TrackController : BasicController<TrackService, TrackModel, TrackViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";
        TrackService service;

        public TrackController(TrackService service) : base(service, ApiVersion)
        {
            this.service = service;
        }

        [HttpGet("search")]
        public IActionResult GetSearch(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")] List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                var result = service.ReadModelSearch(page, size, order, select, keyword, filter);

                //var response =
                //new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE).Ok(null, result.Data, page, size, result.Count, result.Data.Count, result.Order, result.Selected);
                return Ok(new
                {
                    data = result.Data.OrderBy( x => x.Box)
                });
            }
            catch (Exception e)
            {
                var response = new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message).Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, response);
            }
        }
    }
}
