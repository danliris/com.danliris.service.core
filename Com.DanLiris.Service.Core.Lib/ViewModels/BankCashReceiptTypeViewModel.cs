using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services.BankCashReceiptType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class BankCashReceiptTypeViewModel : BasicViewModel, IValidatableObject
    {
        public string Name { get; set; }
        public int COAId { get; set; }
        public string COACode { get; set; }
        public string COAName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IBankCashReceiptTypeService service = (IBankCashReceiptTypeService)validationContext.GetService(typeof(IBankCashReceiptTypeService));

            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Nama harus diisi", new List<string>() { "Name" });
            }
            else if (service.CheckExisting(d => d.Id != Id && d.Name == Name))
            {
                yield return new ValidationResult("Kode sudah ada", new List<string>() { "Code" });
            }

        }
    }
}
