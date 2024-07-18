using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using System.Dynamic;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;
using Com.Moonlay.Models;
using System.Threading.Tasks;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Microsoft.EntityFrameworkCore;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class SupplierService : BasicService<CoreDbContext, Supplier>, IBasicUploadCsvService<SupplierViewModel>, IMap<Supplier, SupplierViewModel>
    {
        private const string UserAgent = "core-product-service";
        protected IIdentityService _IdentityService;
        protected DbSet<Supplier> _DbSet;
        private readonly CoreDbContext _dbContext;
        private readonly string[] ImportAllowed = { "True", "False" };

        public SupplierService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
            //_IdentityService = serviceProvider.GetService<IIdentityService>();
            //_dbContext = serviceProvider.GetService<CoreDbContext>();
        }

        public override Tuple<List<Supplier>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<Supplier> Query = this.DbContext.Suppliers;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "code", "name", "address", "country", "import", "NPWP", "IsPosted"
            };

            Query = Query
                .Select(s => new Supplier
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Address = s.Address,
                    Country = s.Country,
                    Import = s.Import,
                    NPWP = s.NPWP,
                    Active = s.Active
                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_updatedDate", General.DESCENDING);

                Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];
                string TransformKey = General.TransformOrderBy(Key);

                BindingFlags IgnoreCase = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

                Query = OrderType.Equals(General.ASCENDING) ?
                    Query.OrderBy(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b)) :
                    Query.OrderByDescending(b => b.GetType().GetProperty(TransformKey, IgnoreCase).GetValue(b));
            }

            /* Pagination */
            Pageable<Supplier> pageable = new Pageable<Supplier>(Query, Page - 1, Size);
            List<Supplier> Data = pageable.Data.ToList<Supplier>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public SupplierViewModel MapToViewModel(Supplier supplier)
        {
            SupplierViewModel supplierVM = new SupplierViewModel();

            supplierVM._id = supplier.Id;
            supplierVM.UId = supplier.UId;
            supplierVM._deleted = supplier._IsDeleted;
            supplierVM._active = supplier.Active;
            supplierVM._createdDate = supplier._CreatedUtc;
            supplierVM._createdBy = supplier._CreatedBy;
            supplierVM._createAgent = supplier._CreatedAgent;
            supplierVM._updatedDate = supplier._LastModifiedUtc;
            supplierVM._updatedBy = supplier._LastModifiedBy;
            supplierVM._updateAgent = supplier._LastModifiedAgent;
            supplierVM.code = supplier.Code;
            supplierVM.name = supplier.Name;
            supplierVM.address = supplier.Address;
            supplierVM.country = supplier.Country;
            supplierVM.contact = supplier.Contact;
            supplierVM.PIC = supplier.PIC;
            supplierVM.import = supplier.Import;
            supplierVM.NPWP = supplier.NPWP;
            supplierVM.serialNumber = supplier.SerialNumber;
            supplierVM.IsPosted = supplier.Active;

            return supplierVM;
        }

        public Supplier MapToModel(SupplierViewModel supplierVM)
        {
            Supplier supplier = new Supplier();

            supplier.Id = supplierVM._id;
            supplier.UId = supplierVM.UId;
            supplier._IsDeleted = supplierVM._deleted;
            supplier.Active = supplierVM._active;
            supplier._CreatedUtc = supplierVM._createdDate;
            supplier._CreatedBy = supplierVM._createdBy;
            supplier._CreatedAgent = supplierVM._createAgent;
            supplier._LastModifiedUtc = supplierVM._updatedDate;
            supplier._LastModifiedBy = supplierVM._updatedBy;
            supplier._LastModifiedAgent = supplierVM._updateAgent;
            supplier.Code = supplierVM.code;
            supplier.Name = supplierVM.name;
            supplier.Address = supplierVM.address;
            supplier.Country = supplierVM.country;
            supplier.Contact = supplierVM.contact;
            supplier.PIC = supplierVM.PIC;
            supplier.Import = !Equals(supplierVM.import, null) ? Convert.ToBoolean(supplierVM.import) : null; /* Check Null */
            supplier.NPWP = supplierVM.NPWP;
            supplier.SerialNumber = supplierVM.serialNumber;
            supplier.Active = supplierVM.IsPosted;

            return supplier;
        }

        public async Task<int> SupplierPost(List<SupplierViewModel> supplier, string username)
        {
            int Updated = 1;
            var Ids = supplier.Select(d => d._id).ToList();
            var listData = this.DbContext.Suppliers.Where(m => Ids.Contains(m.Id) && !m._IsDeleted).ToList();

            listData.ForEach(async m =>
            {
                Updated = await supplierUpdated(m, username);

            });

            return Updated;
        }

        public async Task<int> supplierUpdated(Supplier model, string username)
        {


            model.Active = true;
            model.FlagForUpdate("dev2", UserAgent);


            DbContext.Suppliers.Update(model);

            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> supplierNonActive(int Id, string username)
        {

            var model = this.DbContext.Suppliers.FirstOrDefault(x => x.Id == Id);

            model.Active = false;
            model.FlagForUpdate(username, UserAgent);

            DbContext.Suppliers.Update(model);
            return await DbContext.SaveChangesAsync();
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Nama Supplier", "Alamat", "Negara", "Kontak", "PIC", "Import", "NPWP", "Serial Number"
        };

        public List<string> CsvHeader => Header;

        public sealed class SupplierMap : ClassMap<SupplierViewModel>
        {
            public SupplierMap()
            {
                
                Map(s => s.code).Index(0);
                Map(s => s.name).Index(1);
                Map(s => s.address).Index(2);
                Map(s => s.country).Index(3);
                Map(s => s.contact).Index(4);
                Map(s => s.PIC).Index(5);
                Map(s => s.import).Index(6).TypeConverter<StringConverter>();
                Map(s => s.NPWP).Index(7);
                Map(s => s.serialNumber).Index(8);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<SupplierViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (SupplierViewModel supplierVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(supplierVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != supplierVM && d.code.Equals(supplierVM.code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(supplierVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }

                if(string.IsNullOrWhiteSpace(supplierVM.import))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Import tidak boleh kosong, ");
                }
                else if(!ImportAllowed.Any(i => i.Equals(supplierVM.import, StringComparison.CurrentCultureIgnoreCase)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Import harus diisi dengan True atau False, ");
                }

                if(string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(supplierVM.code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    supplierVM.import = Convert.ToBoolean(supplierVM.import);
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode", supplierVM.code);
                    Error.Add("Nama Supplier", supplierVM.name);
                    Error.Add("Alamat", supplierVM.address);
                    Error.Add("Negara", supplierVM.country);
                    Error.Add("Kontak", supplierVM.code);
                    Error.Add("PIC", supplierVM.PIC);
                    Error.Add("Import", supplierVM.import);
                    Error.Add("NPWP", supplierVM.NPWP);
                    Error.Add("Serial Number", supplierVM.serialNumber);
                    Error.Add("Error", ErrorMessage);

                    ErrorList.Add(Error);
                }
            }

            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }

       
    }
}