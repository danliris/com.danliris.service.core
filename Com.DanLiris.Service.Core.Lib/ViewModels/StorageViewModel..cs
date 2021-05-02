using Com.DanLiris.Service.Core.Lib.Helpers;
using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class StorageViewModel : BasicViewModelOld
    {

        public string code { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public StorageUnitViewModel unit { get; set; }
    }

    public class StorageUnitViewModel
    {
        public int? _id { get; set; }

        public string name { get; set; }

        public DivisionViewModel division { get; set; }
    }
}
