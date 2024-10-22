using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class XLSGarmentSupplierViewModel 
	{
		public DateTimeOffset createddate { get; set; }
		public string code { get; set; }
		public string name { get; set; }
		public string address { get; set; }
		public string country { get; set; }
		public string import { get; set; }
		public string NPWP { get; set; }
		public string contact { get; set; }
		public string PIC { get; set; }
		public string usevat { get; set; }
		public string usetax { get; set; }
		public string taxname { get; set; }
		public double? taxrate { get; set; }
		public string serialNumber { get; set; }
		public string description { get; set; }
		public string Aktif { get; set; }
	}
}
