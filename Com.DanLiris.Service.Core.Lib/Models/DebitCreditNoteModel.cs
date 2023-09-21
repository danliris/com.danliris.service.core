using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class DebitCreditNoteModel : StandardEntity, IValidatableObject
    {
        public string TypeDCN { get; set; }
        public string ItemTypeDCN { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.TypeDCN))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "TypeDCN" }));

            if (string.IsNullOrWhiteSpace(this.ItemTypeDCN))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "ItemTypeDCN" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                DebitCreditNoteService service = (DebitCreditNoteService)validationContext.GetService(typeof(DebitCreditNoteService));

                if (service.DbContext.Set<DebitCreditNoteModel>().Count(r => r._IsDeleted.Equals(false) && r.TypeDCN == this.TypeDCN && r.ItemTypeDCN.Equals(this.ItemTypeDCN)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "DebitCreditNote" }));
            }



            return validationResult;
        }
    }
}
