using Com.Danliris.Service.Core.Mongo.MongoModels;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.DanLiris.Service.Core.Lib.Models
{
    public class Supplier : StandardEntity, IValidatableObject
    {
        public Supplier()
        {
        }

        public Supplier(SupplierMongo mongoSupplier)
        {
            Active = mongoSupplier._active;
            Address = mongoSupplier.address;
            Code = mongoSupplier.code;
            Contact = mongoSupplier.contact;
            Import = mongoSupplier.import;
            UId = mongoSupplier._id.ToString();
            Name = mongoSupplier.name;
            PIC = mongoSupplier.PIC;
            NPWP = mongoSupplier.NPWP;
            SerialNumber = mongoSupplier.serialNumber;
            _CreatedAgent = mongoSupplier._createAgent;
            _CreatedBy = mongoSupplier._createdBy;
            _CreatedUtc = mongoSupplier._createdDate;
            _DeletedAgent = mongoSupplier._deleted ? mongoSupplier._updateAgent : "";
            _DeletedBy = mongoSupplier._deleted ? mongoSupplier._updatedBy : "";
            _DeletedUtc = mongoSupplier._deleted ? mongoSupplier._updatedDate : new DateTime();
            _IsDeleted = mongoSupplier._deleted;
            _LastModifiedAgent = mongoSupplier._updateAgent;
            _LastModifiedBy = mongoSupplier._updatedBy;
            _LastModifiedUtc = mongoSupplier._updatedDate;
        }

        [MaxLength(255)]
        public string UId { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(3000)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Contact { get; set; }

        [StringLength(500)]
        public string PIC { get; set; }

        public bool? Import { get; set; }

        [StringLength(100)]
        public string NPWP { get; set; }

        [StringLength(500)]
        public string SerialNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(this.Code))
                validationResult.Add(new ValidationResult("Code is required", new List<string> { "code" }));

            if (string.IsNullOrWhiteSpace(this.Name))
                validationResult.Add(new ValidationResult("Name is required", new List<string> { "name" }));

            if (this.Import.Equals(null))
                this.Import = false;

            if (validationResult.Count.Equals(0))
            {
                /* Service Validation */
                SupplierService service = (SupplierService)validationContext.GetService(typeof(SupplierService));

                if (service.DbContext.Set<Supplier>().Count(r => r._IsDeleted.Equals(false) && r.Id != this.Id && r.Code.Equals(this.Code)) > 0) /* Code Unique */
                    validationResult.Add(new ValidationResult("Code already exists", new List<string> { "code" }));
            }

            return validationResult;
        }
    }
}
