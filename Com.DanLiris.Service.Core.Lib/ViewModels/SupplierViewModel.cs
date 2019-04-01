using System;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class SupplierViewModel
    {
        public int _id { get; set; }
        public string UId { get; set; }

        public bool _deleted { get; set; }

        public bool _active { get; set; }

        public DateTime _createdDate { get; set; }

        public string _createdBy { get; set; }

        public string _createAgent { get; set; }

        public DateTime _updatedDate { get; set; }

        public string _updatedBy { get; set; }

        public string _updateAgent { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public string contact { get; set; }

        public string PIC { get; set; }

        /* Bool */
        public dynamic import { get; set; }

        public string NPWP { get; set; }

        public string serialNumber { get; set; }
    }
}
