using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class FabricType : StandardEntity, IValidatableObject
    {
        
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "Name" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                FabricTypeService service = (FabricTypeService)validationContext.GetService(typeof(FabricTypeService));

                if (service.DbContext.Set<FabricType>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Name.Equals(this.Name)) > 0) /* Name Unique */
                    validationResult.Add(new ValidationResult("Name already exists", new List<string> { "Name" }));
            }

            return validationResult;
        }
    }
}
