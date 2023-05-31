using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class TrackViewModel :  BasicViewModel
    {
        
        public string Type { get; set; }
        public string Name { get; set; }
        public string Box { get; set; }
        public string Concat { get; set; }
    }
}
