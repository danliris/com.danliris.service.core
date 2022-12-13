using System;
using Com.DanLiris.Service.Core.Lib.Helpers;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class MenuViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string Menu { get; set; }
        public string SubMenu { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
    }
}
