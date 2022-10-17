using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class ProductTextileViewModel : BasicViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public ProductTextileUomViewModel UOM {get; set;}
        public string Description { get; set; }

        public class ProductTextileUomViewModel
        {
            public int? Id { get; set; }

            public string Unit { get; set; }
        }

    }
}
