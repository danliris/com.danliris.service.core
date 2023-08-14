using Com.DanLiris.Service.Core.Lib.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class ProductViewModel : BasicViewModel
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public dynamic Price { get; set; }
        
        public ProductCurrencyViewModel Currency { get; set; }

        public ProductSPPPropertyViewModel SPPProperties { get; set; }

        public string Description { get; set; }

        public ProductUomViewModel UOM { get; set; }
        
        public string Tags { get; set; }
        public bool IsPosted { get; set; }
        public bool IsPriceChange { get; set; }
        public string EditReason { get; set; }
    }

    public class MonitoringProductViewModel : BasicViewModel
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public dynamic Price { get; set; }
        public dynamic PriceOrigin { get; set; }

        public decimal DiffPrice {get; set;}
        public decimal DiffPricePercentage {get; set;}

        public string CurrencyCode { get; set; }

        public ProductSPPPropertyViewModel SPPProperties { get; set; }

        public string Description { get; set; }

        public string UOMUnit { get; set; }

        public string Tags { get; set; }
        public bool IsPosted { get; set; }
        public bool IsPriceChange { get; set; }
        public string EditReason { get; set; }
        public DateTimeOffset DateIn { get; set; }
        public DateTimeOffset DateChange { get; set; }
    }

    public class ProductSPPPropertyViewModel
    {
        public int BuyerId { get; set; }

        [MaxLength(250)]
        public string BuyerName { get; set; }

        [MaxLength(250)]
        public string BuyerAddress { get; set; }

        [MaxLength(250)]
        public string ColorName { get; set; }

        [MaxLength(300)]
        public string Construction { get; set; }

        [MaxLength(250)]
        public string DesignCode { get; set; }

        [MaxLength(250)]
        public string DesignNumber { get; set; }

        [MaxLength(100)]
        public string Grade { get; set; }

        public double Length { get; set; }

        [MaxLength(250)]
        public string Lot { get; set; }

        public OrderTypeViewModel OrderType { get; set; } 

        public int ProductionOrderId { get; set; }

        [MaxLength(25)]
        public string ProductionOrderNo { get; set; }
        
        public double Weight { get; set; }
       
    }

    public class ProductCurrencyViewModel
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public string Symbol { get; set; }
    }

    public class ProductUomViewModel
    {
        public int? Id { get; set; }

        public string Unit { get; set; }
    }

    public class ProductDesignViewModel
    {
        public string Code { get; set; }

        public string Number { get; set; }
    }
}
