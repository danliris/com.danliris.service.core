﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers
{ 

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/garment-buyer-brands")]
    public class GarmentBuyerBrandController : BasicController<GarmentBuyerBrandService, GarmentBuyerBrand, GarmentBuyerBrandViewModel, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";

        public GarmentBuyerBrandController(GarmentBuyerBrandService service) : base(service, ApiVersion)
        {
        }
    }
}