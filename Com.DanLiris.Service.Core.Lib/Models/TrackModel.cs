using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class TrackModel :StandardEntity, IValidatableObject
    {
        [StringLength(255)]
        public string Type { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public string Box { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Type))
                validationResult.Add(new ValidationResult("Type is required", new List<string> { "type" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (string.IsNullOrWhiteSpace(this.Box))
                validationResult.Add(new ValidationResult("Box is required", new List<string> { "box" }));

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                TrackService service = (TrackService)validationContext.GetService(typeof(TrackService));

                if (service.DbContext.Set<TrackModel>().Count(r => r._IsDeleted.Equals(false) && r.Type == this.Type && r.Name.Equals(this.Name) && r.Box.Equals(this.Box)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "track" }));
            }
            return validationResult;
        }
    }
}
