using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles
{
    public class Menus : StandardEntity, IValidatableObject
    {
        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }
        [StringLength(500)]
        public string Menu { get; set; }
        [StringLength(500)]
        public string SubMenu { get; set; }
        [StringLength(500)]
        public string MenuName { get; set; }
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));
            if (string.IsNullOrWhiteSpace(this.Menu))
                validationResult.Add(new ValidationResult("Menu is required", new List<string> { "menu" }));
            if (string.IsNullOrWhiteSpace(this.SubMenu))
                validationResult.Add(new ValidationResult("SubMenu is required", new List<string> { "submenu" }));
            if (string.IsNullOrWhiteSpace(this.MenuName))
                validationResult.Add(new ValidationResult("MenuName is required", new List<string> { "menuname" }));

            return validationResult;
        }
    }
}
