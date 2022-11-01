using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class ProductTextile : StandardEntity, IValidatableObject
    {
        [StringLength(255)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public int? UomId { get; set; }
        [StringLength(255)]
        public string UomUnit { get; set; }
        [StringLength(50)]
        public string BuyerType { get; set; }
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();
            return validationResult;
        }
    }
}
