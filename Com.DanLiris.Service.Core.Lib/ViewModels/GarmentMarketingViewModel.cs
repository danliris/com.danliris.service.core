using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services.GarmentMarketing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.DanLiris.Service.Core.Lib.ViewModels
{
    public class GarmentMarketingViewModel : BasicViewModel, IValidatableObject
    {
        public string Name { get; set; }

        public string ResponsibleName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IGarmentMarketingService service = (IGarmentMarketingService)validationContext.GetService(typeof(IGarmentMarketingService));


            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Nama Marketing harus diisi", new List<string>() { "Name" });
            }
            else if (service.CheckExisting(d => d.Id != Id && d.Name == Name))
            {
                yield return new ValidationResult("Nama Marketing sudah ada", new List<string>() { "Name" });
            }


            if (string.IsNullOrWhiteSpace(ResponsibleName))
            {
                yield return new ValidationResult("Penanguung Jawab harus diisi", new List<string>() { "ResponsibleName" });
            }

        }
    }
}
