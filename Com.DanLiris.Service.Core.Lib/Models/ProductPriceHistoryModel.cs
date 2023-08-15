using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class ProductPriceHistoryModel : StandardEntity
    {
        public ProductPriceHistoryModel()
        {


        }

        public ProductPriceHistoryModel(string code, string name, decimal priceOrigin, decimal price, int? currencyId, string currencyCode, string currencySymbol, string editReason, int? uomId, string uomUnit, int productId, DateTime dateBefore)
        {
            Code = code;
            Name = name;
            PriceOrigin = priceOrigin;
            Price = price;
            CurrencyId = currencyId;
            CurrencyCode = currencyCode;
            CurrencySymbol = currencySymbol;
            EditReason = editReason;
            UomId = uomId;
            UomUnit = uomUnit;
            ProductId = productId;
            DateBefore = dateBefore;

        }
        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public decimal PriceOrigin { get; set; }
        public decimal Price { get; set; }

        public int? CurrencyId { get; set; }

        [StringLength(255)]
        public string CurrencyCode { get; set; }
        [StringLength(255)]
        public string CurrencySymbol { get; set; }

        [StringLength(1000)]
        public string EditReason { get; set; }

        public int? UomId { get; set; }

        [StringLength(500)]
        public string UomUnit { get; set; }

        public int ProductId { get; set; }
        public DateTime DateBefore { get; set; }

        
    }
}
