using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.GarmentWareHouse;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.DanLiris.Service.Core.Test.Utils;
using Com.DanLiris.Service.Core.WebApi.Controllers.v1.BasicControllers;

namespace Com.DanLiris.Service.Core.Test.Controllers.GarmentWareHouse
{
    public class BasicTest : BaseControllerTest<GarmentWareHouseController, GarmentWareHouseModel, GarmentWareHouseViewModel, IGarmentWareHouseService>
    {
        public BasicTest()
        {
        }
    }
}
