using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.GarmentMarketing;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Utils;
using Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentMarketing
{
    public class BasicTest : BaseControllerTest<GarmentMarketingController, GarmentMarketingModel, GarmentMarketingViewModel, IGarmentMarketingService>
    {
        public BasicTest()
        {
        }
    }
}
