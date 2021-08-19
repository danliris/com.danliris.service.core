using Microsoft.AspNetCore.Mvc;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Lib;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/divisions")]
    public class DivisionsController : BasicController<DivisionService, Division, DivisionViewModel, CoreDbContext>
    {
        private new static readonly string ApiVersion = "1.0";

        public DivisionsController(DivisionService service) : base(service, ApiVersion)
        {
        }

        [HttpGet("division-group")]
        public IActionResult GetByDivisionName(int Page = 1, int Size = 25, string Order = "{}", [Bind(Prefix = "Select[]")] List<string> Select = null, string Keyword = "", string Filter = "{}")
        {
            try
            {
                Tuple<List<DivisionGroupViewModel>, int, Dictionary<string, string>, List<string>> Data = Service.ReadModelByDivisionName(Page, Size, Order, Select, Keyword, Filter);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data.Item1, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

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
