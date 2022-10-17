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

namespace Com.DanLiris.Service.Core.WebApi.Controllers.v1.UploadControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/master/upload-product-textile")]
    public class ProductTextileUploadController : BasicUploadController<ProductTextileService, ProductTextile, ProductTextileViewModel, ProductTextileService.ProductTextileMap, CoreDbContext>
    {
        private static readonly string ApiVersion = "1.0";
        public ProductTextileUploadController(ProductTextileService service) : base(service, ApiVersion)
        {
        }
    }
}
