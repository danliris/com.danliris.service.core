﻿namespace Com.Danliris.Service.Core.Mongo.MongoModels
{
    public class SupplierMongo : MongoBaseModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public string PIC { get; set; }
        public bool import { get; set; }
        public string NPWP { get; set; }
        public string serialNumber { get; set; }
        public bool useIncomeTax { get; set; }
    }
}
