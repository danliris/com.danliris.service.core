using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using static Com.DanLiris.Service.Core.Lib.ViewModels.ProductTextileViewModel;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ProductTextileService : BasicService<CoreDbContext, ProductTextile>, IBasicUploadCsvService<ProductTextileViewModel>, IMap<ProductTextile, ProductTextileViewModel>
    {
        private const string UserAgent = "core-product-textile-service";

        public ProductTextileService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
        }

        public override Tuple<List<ProductTextile>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<ProductTextile> Query = this.DbContext.ProductTextiles.AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Description", "BuyerType"
            };

            

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
            //Pageable<Product> pageable = new Pageable<Product>(Query, Page - 1, Size);

            var totalData = Query.Count();
            Query = Query.Skip((Page - 1) * Size).Take(Size);

            List<ProductTextile> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "Kode", "Nama", "Satuan", "Keterangan", "Jenis"
        };

        public sealed class ProductTextileMap : ClassMap<ProductTextileViewModel>
        {
            public ProductTextileMap()
            {
                Map(p => p.Code).Index(0);
                Map(p => p.Name).Index(1);
                Map(p => p.UOM.Unit).Index(2);
                Map(p => p.Description).Index(3);
                Map(p => p.BuyerType).Index(4);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<ProductTextileViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            Currency currency = null;
            Uom uom = null;

            foreach (ProductTextileViewModel productTVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(productTVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productTVM && d.Code.Equals(productTVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productTVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productTVM && d.Name.Equals(productTVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productTVM.UOM.Unit))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak boleh kosong, ");
                }

                

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    uom = this.DbContext.Set<Uom>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Unit.Equals(productTVM.UOM.Unit));

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(productTVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(productTVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }


                    if (uom == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak terdaftar dalam master Satuan, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                   
                    productTVM.UOM.Id = uom.Id;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Barang", productTVM.Code);
                    Error.Add("Nama Barang", productTVM.Name);
                    Error.Add("Satuan", productTVM.UOM.Unit);
                   
                    Error.Add("Keterangan", productTVM.Description);
                    Error.Add("Jenis", productTVM.BuyerType);
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

        public ProductTextile MapToModel(ProductTextileViewModel productTVM)
        {
            ProductTextile productT = new ProductTextile
            {
                Id = productTVM.Id,
                _IsDeleted = productTVM._IsDeleted,
                Active = productTVM.Active,
                _CreatedUtc = productTVM._CreatedUtc,
                _CreatedBy = productTVM._CreatedBy,
                _CreatedAgent = productTVM._CreatedAgent,
                _LastModifiedUtc = productTVM._LastModifiedUtc,
                _LastModifiedBy = productTVM._LastModifiedBy,
                _LastModifiedAgent = productTVM._LastModifiedAgent,
                Code = productTVM.Code,
                Name = productTVM.Name,
                UomId = productTVM.UOM.Id,
                UomUnit = productTVM.UOM.Unit,
                Description = productTVM.Description,
                BuyerType = productTVM.BuyerType,
            };
                

            return productT;
        }

        public ProductTextileViewModel MapToViewModel(ProductTextile productT)
        {
            ProductTextileViewModel productTVM = new ProductTextileViewModel
            {
                Id = productT.Id,
                _IsDeleted = productT._IsDeleted,
                Active = productT.Active,
                _CreatedUtc = productT._CreatedUtc,
                _CreatedBy = productT._CreatedBy,
                _CreatedAgent = productT._CreatedAgent,
                _LastModifiedUtc = productT._LastModifiedUtc,
                _LastModifiedBy = productT._LastModifiedBy,
                _LastModifiedAgent = productT._LastModifiedAgent,
                Code = productT.Code,
                Name = productT.Name,
                Description = productT.Description,
                UOM = new ProductTextileUomViewModel
                {
                    Id = productT.UomId,
                    Unit = productT.UomUnit
                },
                BuyerType = productT.BuyerType
                
            };

            return productTVM;
        }


    }
}
