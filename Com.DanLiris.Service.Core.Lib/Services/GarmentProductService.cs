using Com.DanLiris.Service.Core.Lib.Models;
using Com.Moonlay.NetCore.Lib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using CsvHelper.Configuration;
using System.Dynamic;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentProductService : BasicService<CoreDbContext, GarmentProduct>, IBasicUploadCsvService<GarmentProductViewModel>, IMap<GarmentProduct, GarmentProductViewModel>
    {
        public GarmentProductService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentProduct>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentProduct> Query = this.DbContext.GarmentProducts;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name", "UomUnit", "tags"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code", "Name", "UOM", "Tags", "_LastModifiedUtc"
            };

            Query = Query
                .Select(p => new GarmentProduct
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    UomId = p.UomId,
                    UomUnit = p.UomUnit,
                    Tags = p.Tags,
                    _LastModifiedUtc = p._LastModifiedUtc
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
            Pageable<GarmentProduct> pageable = new Pageable<GarmentProduct>(Query, Page - 1, Size);
            List<GarmentProduct> Data = pageable.Data.ToList<GarmentProduct>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentProductViewModel MapToViewModel(GarmentProduct garmentProduct)
        {
            GarmentProductViewModel garmentProductVM = new GarmentProductViewModel
            {
                Id = garmentProduct.Id,
                _IsDeleted = garmentProduct._IsDeleted,
                Active = garmentProduct.Active,
                _CreatedUtc = garmentProduct._CreatedUtc,
                _CreatedBy = garmentProduct._CreatedBy,
                _CreatedAgent = garmentProduct._CreatedAgent,
                _LastModifiedUtc = garmentProduct._LastModifiedUtc,
                _LastModifiedBy = garmentProduct._LastModifiedBy,
                _LastModifiedAgent = garmentProduct._LastModifiedAgent,
                Code = garmentProduct.Code,
                Name = garmentProduct.Name,
                
                UOM = new GarmentProductUomViewModel
                {
                    Id = garmentProduct.UomId,
                    Unit = garmentProduct.UomUnit
                },
                Tags = garmentProduct.Tags,
                Const = garmentProduct.Const,
                Yarn = garmentProduct.Yarn,
                Width = garmentProduct.Width,
                Remark = garmentProduct.Remark
            };

            return garmentProductVM;
        }

        public GarmentProduct MapToModel(GarmentProductViewModel garmentProductVM)
        {
            GarmentProduct garmentProduct = new GarmentProduct
            {
                Id = garmentProductVM.Id,
                _IsDeleted = garmentProductVM._IsDeleted,
                Active = garmentProductVM.Active,
                _CreatedUtc = garmentProductVM._CreatedUtc,
                _CreatedBy = garmentProductVM._CreatedBy,
                _CreatedAgent = garmentProductVM._CreatedAgent,
                _LastModifiedUtc = garmentProductVM._LastModifiedUtc,
                _LastModifiedBy = garmentProductVM._LastModifiedBy,
                _LastModifiedAgent = garmentProductVM._LastModifiedAgent,
                Code = garmentProductVM.Code,
                Name = garmentProductVM.Name,
                Tags = garmentProductVM.Tags,
                Const = garmentProductVM.Const,
                Yarn = garmentProductVM.Yarn,
                Width = garmentProductVM.Width,
                Remark = garmentProductVM.Remark
            };
            if (!Equals(garmentProductVM.UOM, null))
            {
                garmentProduct.UomId = garmentProductVM.UOM.Id;
                garmentProduct.UomUnit = garmentProductVM.UOM.Unit;
            }
            else
            {
                garmentProduct.UomId = null;
                garmentProduct.UomUnit = null;
            }

            

            return garmentProduct;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode Barang", "Nama Barang", "Satuan", "Const", "Yarn", "Width", "Tags", "Keterangan"
        };

        public List<string> CsvHeader => Header;

        public sealed class GarmentProductMap : ClassMap<GarmentProductViewModel>
        {
            public GarmentProductMap()
            {
                Map(p => p.Code).Index(0);
                Map(p => p.Name).Index(1);
                Map(p => p.UOM.Unit).Index(2);
                Map(p => p.Const).Index(3);
                Map(p => p.Yarn).Index(4);
                Map(p => p.Width).Index(5);
                Map(p => p.Tags).Index(6);
                Map(p => p.Remark).Index(7);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<GarmentProductViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            Uom uom = null;

            foreach (GarmentProductViewModel garmentProductVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(garmentProductVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != garmentProductVM && d.Code.Equals(garmentProductVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(garmentProductVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != garmentProductVM && d.Name.Equals(garmentProductVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(garmentProductVM.UOM.Unit))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak boleh kosong, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    uom = this.DbContext.Set<Uom>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Unit.Equals(garmentProductVM.UOM.Unit));

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(garmentProductVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(garmentProductVM.Name)))
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
                    garmentProductVM.UOM.Id = uom.Id;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Barang", garmentProductVM.Code);
                    Error.Add("Nama Barang", garmentProductVM.Name);
                    Error.Add("Satuan", garmentProductVM.UOM.Unit);
                    Error.Add("Const", garmentProductVM.Const);
                    Error.Add("Yarn", garmentProductVM.Yarn);
                    Error.Add("Width", garmentProductVM.Width);
                    Error.Add("Tags", garmentProductVM.Tags);
                    Error.Add("Remark", garmentProductVM.Remark);
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

        public List<GarmentProduct> GetByIds(List<string> ids)
        {
            return this.DbSet.Where(p => ids.Contains(p.Id.ToString()) && p._IsDeleted == false)
                .ToList();
        }
    }
}