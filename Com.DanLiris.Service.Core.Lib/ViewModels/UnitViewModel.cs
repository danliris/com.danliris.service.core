﻿using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class UnitViewModel  : BasicViewModel
    {
        public string Code { get; set; }

        public DivisionViewModel Division { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
