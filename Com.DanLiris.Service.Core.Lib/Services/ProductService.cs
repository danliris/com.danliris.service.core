using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ProductService : BasicService<CoreDbContext, Product>, IBasicUploadCsvService<ProductViewModel>, IMap<Product, ProductViewModel>
    {
        private const string UserAgent = "core-product-service";
        protected IIdentityService _IdentityService;
        protected DbSet<Product> _DbSet;
        private readonly CoreDbContext _dbContext;
        public ProductService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
            //_DbContext = dbContext;
            //_DbSet = dbContext.Set<Product>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
            _dbContext = serviceProvider.GetService<CoreDbContext>();
        }

        public override Tuple<List<Product>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Product> Query = this.DbContext.Products.AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Currency",  "Price", "Tags", "_LastModifiedUtc", "IsPosted"
            };

            //Query = Query
            //    .Select(p => new Product
            //    {
            //        Id = p.Id,
            //        Code = p.Code,
            //        Name = p.Name,
            //        UomId = p.UomId,
            //        UomUnit = p.UomUnit,
            //        CurrencyId = p.CurrencyId,
            //        CurrencyCode = p.CurrencyCode,
            //        CurrencySymbol = p.CurrencySymbol,
            //        Price = p.Price,
            //        Tags = p.Tags,
            //        //SPPProperties = p.SPPProperties == null ? new ProductSPPProperty() : new ProductSPPProperty()
            //        //{
            //        //    ColorName = p.SPPProperties.ColorName,
            //        //    DesignCode = p.SPPProperties.DesignCode,
            //        //    DesignNumber = p.SPPProperties.DesignNumber,
            //        //    ProductionOrderId = p.SPPProperties.ProductionOrderId,
            //        //    ProductionOrderNo = p.SPPProperties.ProductionOrderNo,
            //        //    BuyerAddress = p.SPPProperties.BuyerAddress,
            //        //    BuyerId = p.SPPProperties.BuyerId,
            //        //    BuyerName = p.SPPProperties.BuyerName,
            //        //    Weight = p.SPPProperties.Weight,
            //        //    Construction = p.SPPProperties.Construction,
            //        //    Grade = p.SPPProperties.Grade,
            //        //    Length = p.SPPProperties.Length,
            //        //    Lot = p.SPPProperties.Lot,
            //        //    OrderTypeCode = p.SPPProperties.OrderTypeCode,
            //        //    OrderTypeId = p.SPPProperties.OrderTypeId,
            //        //    OrderTypeName = p.SPPProperties.OrderTypeName
            //        //},
            //        _LastModifiedUtc = p._LastModifiedUtc
            //    }).AsNoTracking();

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

            List<Product> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public Tuple<List<Product>, int, Dictionary<string, string>, List<string>> ReadModelNullTags(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Product> Query = this.DbContext.Products.Where(x => string.IsNullOrEmpty(x.Tags) || string.IsNullOrWhiteSpace(x.Tags)).AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Currency",  "Price", "Tags", "_LastModifiedUtc"
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

            List<Product> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public Tuple<List<Product>, int, Dictionary<string, string>, List<string>> ReadModelNullPrice(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            List<string> filterTags = new List<string>(){ "", "Dye Stuff Printing", "MATERIAL" } ;
            IQueryable<Product> Query = this.DbContext.Products.Where(x => x.Price == 0 && filterTags.Contains(x.Tags) ).AsNoTracking();
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
                "Id", "Code", "Name", "UOM", "Currency",  "Price", "Tags", "_LastModifiedUtc"
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

            List<Product> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public ProductViewModel MapToViewModel(Product product)
        {
            ProductViewModel productVM = new ProductViewModel
            {
                Id = product.Id,
                UId = product.UId,
                _IsDeleted = product._IsDeleted,
                Active = product.Active,
                _CreatedUtc = product._CreatedUtc,
                _CreatedBy = product._CreatedBy,
                _CreatedAgent = product._CreatedAgent,
                _LastModifiedUtc = product._LastModifiedUtc,
                _LastModifiedBy = product._LastModifiedBy,
                _LastModifiedAgent = product._LastModifiedAgent,
                Code = product.Code,
                Name = product.Name,
                Price = product.Price,

                Currency = new ProductCurrencyViewModel
                {
                    Id = product.CurrencyId,
                    Code = product.CurrencyCode,
                    Symbol = product.CurrencySymbol
                },
                Description = product.Description,
                UOM = new ProductUomViewModel
                {
                    Id = product.UomId,
                    Unit = product.UomUnit
                },
                Tags = product.Tags,
                SPPProperties = product.SPPProperties == null ? null : new ProductSPPPropertyViewModel()
                {
                    BuyerAddress = product.SPPProperties.BuyerAddress,
                    BuyerId = product.SPPProperties.BuyerId,
                    BuyerName = product.SPPProperties.BuyerName,
                    ColorName = product.SPPProperties.ColorName,
                    Construction = product.SPPProperties.Construction,
                    DesignCode = product.SPPProperties.DesignCode,
                    DesignNumber = product.SPPProperties.DesignNumber,
                    Grade = product.SPPProperties.Grade,
                    Length = product.SPPProperties.Length,
                    Lot = product.SPPProperties.Lot,
                    ProductionOrderId = product.SPPProperties.ProductionOrderId,
                    ProductionOrderNo = product.SPPProperties.ProductionOrderNo,
                    Weight = product.SPPProperties.Weight,
                    OrderType = new OrderTypeViewModel()
                    {
                        Id = product.SPPProperties.OrderTypeId,
                        Code = product.SPPProperties.OrderTypeCode,
                        Name = product.SPPProperties.OrderTypeName
                    }


                },
                IsPosted = product.Active
            };

            return productVM;
        }

        public Product MapToModel(ProductViewModel productVM)
        {
            Product product = new Product
            {
                Id = productVM.Id,
                UId = productVM.UId,
                _IsDeleted = productVM._IsDeleted,
                Active = productVM.Active,
                _CreatedUtc = productVM._CreatedUtc,
                _CreatedBy = productVM._CreatedBy,
                _CreatedAgent = productVM._CreatedAgent,
                _LastModifiedUtc = productVM._LastModifiedUtc,
                _LastModifiedBy = productVM._LastModifiedBy,
                _LastModifiedAgent = productVM._LastModifiedAgent,
                Code = productVM.Code,
                Name = productVM.Name,
                Price = !Equals(productVM.Price, null) ? Convert.ToDecimal(productVM.Price) : 0, /* Check Null */
                SPPProperties = productVM.SPPProperties == null ? null : new ProductSPPProperty()
                {
                    Id = productVM.Id,
                    BuyerAddress = productVM.SPPProperties.BuyerAddress,
                    BuyerId = productVM.SPPProperties.BuyerId,
                    BuyerName = productVM.SPPProperties.BuyerName,
                    ColorName = productVM.SPPProperties.ColorName,
                    Construction = productVM.SPPProperties.Construction,
                    DesignCode = productVM.SPPProperties.DesignCode,
                    DesignNumber = productVM.SPPProperties.DesignNumber,
                    Grade = productVM.SPPProperties.Grade,
                    Length = productVM.SPPProperties.Length,
                    Lot = productVM.SPPProperties.Lot,
                    OrderTypeCode = productVM.SPPProperties.OrderType == null ? null : productVM.SPPProperties.OrderType.Code,
                    OrderTypeId = productVM.SPPProperties.OrderType == null ? 0 : productVM.SPPProperties.OrderType.Id,
                    OrderTypeName = productVM.SPPProperties.OrderType == null ? null : productVM.SPPProperties.OrderType.Name,
                    ProductionOrderId = productVM.SPPProperties.ProductionOrderId,
                    ProductionOrderNo = productVM.SPPProperties.ProductionOrderNo,
                    Weight = productVM.SPPProperties.Weight

                }
            };

            if (!Equals(productVM.Currency, null))
            {
                product.CurrencyId = productVM.Currency.Id;
                product.CurrencyCode = productVM.Currency.Code;
                product.CurrencySymbol = productVM.Currency.Symbol;
            }
            else
            {
                product.CurrencyId = null;
                product.CurrencyCode = null;
            }

            product.Description = productVM.Description;

            if (!Equals(productVM.UOM, null))
            {
                product.UomId = productVM.UOM.Id;
                product.UomUnit = productVM.UOM.Unit;
            }
            else
            {
                product.UomId = null;
                product.UomUnit = null;
            }

            product.Tags = productVM.Tags;

            return product;
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "Kode Barang", "Nama Barang", "Satuan", "Mata Uang", "Harga", "Tags", "Keterangan"
        };

        public sealed class ProductMap : ClassMap<ProductViewModel>
        {
            public ProductMap()
            {
                Map(p => p.Code).Index(0);
                Map(p => p.Name).Index(1);
                Map(p => p.UOM.Unit).Index(2);
                Map(p => p.Currency.Code).Index(3);
                Map(p => p.Price).Index(4).TypeConverter<StringConverter>();
                Map(p => p.Tags).Index(5);
                Map(p => p.Description).Index(6);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<ProductViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            Currency currency = null;
            Uom uom = null;

            foreach (ProductViewModel productVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(productVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.Code.Equals(productVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.Name.Equals(productVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.UOM.Unit))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Currency.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak boleh kosong, ");
                }

                decimal Price = 0;
                if (string.IsNullOrWhiteSpace(productVM.Price))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.Price, out Price))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus numerik, ");
                }
                else if (Price < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus lebih besar dari 0, ");
                }
                else
                {
                    string[] PriceSplit = productVM.Price.Split('.');
                    if (PriceSplit.Count().Equals(2) && PriceSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Harga maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    currency = this.DbContext.Set<Currency>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Code.Equals(productVM.Currency.Code));
                    uom = this.DbContext.Set<Uom>().FirstOrDefault(d => d._IsDeleted.Equals(false) && d.Unit.Equals(productVM.UOM.Unit));

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(productVM.Code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(productVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }

                    if (currency == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak terdaftar dalam master Mata Uang, ");
                    }

                    if (uom == null)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak terdaftar dalam master Satuan, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    productVM.Price = Price;
                    productVM.Currency.Id = currency.Id;
                    productVM.UOM.Id = uom.Id;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Barang", productVM.Code);
                    Error.Add("Nama Barang", productVM.Name);
                    Error.Add("Satuan", productVM.UOM.Unit);
                    Error.Add("Mata Uang", productVM.Currency.Code);
                    Error.Add("Harga", productVM.Price);
                    Error.Add("Tags", productVM.Tags);
                    Error.Add("Keterangan", productVM.Description);
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

        public List<Product> GetByIds(List<string> ids)
        {
            return this.DbSet.Include(x => x.SPPProperties).Where(p => ids.Contains(p.Id.ToString()) && p._IsDeleted == false)
                .ToList();
        }

        public List<Product> GetSimple()
        {
            return DbSet.IgnoreQueryFilters().Select(x => new Product()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();
        }

        public override Task<Product> ReadModelById(int Id)
        {
            //base.DbContext.Set<ProductSPPProperty>().Load();
            return DbSet.Include(x => x.SPPProperties).AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> CreateProduct(PackingModel packings)
        {
            //DbContext.Database.SetCommandTimeout(1000 * 60 * 10);

            var packingProducts = packings.PackingDetails.Select(packingDetail => string.Format("{0}/{1}/{2}/{3}/{4}/{5}", packings.ProductionOrderNo, packings.ColorName, packings.Construction,
                                packingDetail.Lot, packingDetail.Grade, packingDetail.Length) +
                                (string.IsNullOrWhiteSpace(packingDetail.Remark) ? "" : string.Format("/{0}", packingDetail.Remark))).ToList();

            //var productNames = (from packingDetail in packings.PackingDetails
            //                    select )
            //                    .Except((from product in DbSet where product._IsDeleted == false select product.Name));

            var uom = DbContext.UnitOfMeasurements.FirstOrDefault(f => f.Unit.Equals(packings.PackingUom));
            if (uom == null)
            {
                uom = new Uom
                {
                    Active = true,
                    Unit = packings.PackingUom,
                    _IsDeleted = false,
                    _CreatedBy = this.Username,
                    _CreatedUtc = DateTimeOffset.Now.DateTime,
                    _CreatedAgent = UserAgent
                };
                DbContext.UnitOfMeasurements.Add(uom);
                await DbContext.SaveChangesAsync();

                //uomId = uom.Id;
            }

            var tags = string.Format("sales contract #{0}", packings.SalesContractNo);
            CodeGenerator codeGenerator = new CodeGenerator();
            var listProductToCreate = new List<Product>();
            foreach (var packingDetail in packings.PackingDetails)
            {
                var packingProduct = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", packings.ProductionOrderNo, packings.ColorName, packings.Construction, packingDetail.Lot, packingDetail.Grade, packingDetail.Length) + (string.IsNullOrWhiteSpace(packingDetail.Remark) ? "" : string.Format("/{0}", packingDetail.Remark));

                var existingProduct = DbContext.Products.FirstOrDefault(f => f.Name.Equals(packingProduct));

                if (existingProduct == null)
                {
                    var productToCreate = new Product()
                    {
                        Active = true,
                        Code = codeGenerator.GenerateCode(),
                        Name = packingProduct,
                        UomId = uom.Id,
                        UomUnit = packings.PackingUom,
                        Tags = tags,
                        SPPProperties = new ProductSPPProperty()
                        {
                            ProductionOrderId = packings.ProductionOrderId,
                            ProductionOrderNo = packings.ProductionOrderNo,
                            DesignCode = packings.DesignCode,
                            DesignNumber = packings.DesignNumber,
                            OrderTypeId = packings.OrderTypeId,
                            OrderTypeCode = packings.OrderTypeCode,
                            OrderTypeName = packings.OrderTypeName,
                            BuyerId = packings.BuyerId,
                            BuyerName = packings.BuyerName,
                            BuyerAddress = packings.BuyerAddress,
                            ColorName = packings.ColorName,
                            Construction = packings.Construction,
                            Lot = packingDetail.Lot,
                            Grade = packingDetail.Grade,
                            Weight = packingDetail.Weight,
                            Length = packingDetail.Length
                        },
                        _IsDeleted = false,
                        _CreatedBy = this.Username,
                        _CreatedUtc = DateTimeOffset.Now.DateTime,
                        _CreatedAgent = UserAgent
                    };

                    listProductToCreate.Add(productToCreate);
                }

            }

            if (listProductToCreate.Count > 0)
                await DbContext.AddRangeAsync(listProductToCreate);


            var rowAffected = await DbContext.SaveChangesAsync();
            if (rowAffected > 0)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        public Task<List<ProductViewModel>> GetProductByProductionOrderNo(string productionOrderNo)
        {
            var product = DbContext.ProductSPPProperties.Include(x => x.Product).Where(x => x.ProductionOrderNo == productionOrderNo);

            return product.Select(x => new ProductViewModel()
            {
                Active = x.Product.Active,
                Code = x.Product.Code,
                Currency = new ProductCurrencyViewModel()
                {
                    Code = x.Product.CurrencyCode,
                    Id = x.Product.CurrencyId,
                    Symbol = x.Product.CurrencySymbol
                },
                Description = x.Product.Description,
                Id = x.Product.Id,
                Name = x.Product.Name,
                Price = x.Product.Price,
                Tags = x.Product.Tags,
                UId = x.Product.UId,
                UOM = new ProductUomViewModel()
                {
                    Id = x.Product.UomId,
                    Unit = x.Product.UomUnit
                },
                _LastModifiedUtc = x.Product._LastModifiedUtc,
                SPPProperties = new ProductSPPPropertyViewModel()
                {
                    BuyerAddress = x.BuyerAddress,
                    BuyerId = x.BuyerId,
                    BuyerName = x.BuyerName,
                    ColorName = x.ColorName,
                    Construction = x.Construction,
                    DesignCode = x.DesignCode,
                    DesignNumber = x.DesignNumber,
                    Grade = x.Grade,
                    Length = x.Length,
                    Lot = x.Lot,
                    OrderType = new OrderTypeViewModel()
                    {
                        Code = x.OrderTypeCode,
                        Id = x.OrderTypeId,
                        Name = x.OrderTypeName
                    },
                    ProductionOrderId = x.ProductionOrderId,
                    ProductionOrderNo = x.ProductionOrderNo,
                    Weight = x.Weight
                }
            }).ToListAsync();
        }

        public Task<Product> GetProductByName(string productName)
        {
            return DbSet.FirstOrDefaultAsync(f => f.Name.Equals(productName));
        }

        public Task<Product> GetProductForSpinning(int Id)
        {
            return DbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }
        public MemoryStream DownloadTemplate()
        {
            IQueryable<Product> Query = (from a in this.DbContext.Products
                                        where a.Price > 0
                                        select a).OrderByDescending(s=>s._CreatedUtc);

            DataTable result = new DataTable();

 
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Barang", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Create", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Update", DataType = typeof(String) });
            int idx = 0;
            foreach (var item in Query)
            {
                idx++;
                result.Rows.Add(item.Code, item.Name,  item.Price,  item.CurrencyCode, item.UomUnit, item.Description, item._CreatedUtc.ToString(),item._LastModifiedUtc.ToString());
            }
            ExcelPackage package = new ExcelPackage();
          

            var sheet = package.Workbook.Worksheets.Add("Report");
            sheet.Cells["A1"].LoadFromDataTable(result, true, OfficeOpenXml.Table.TableStyles.Light16);
            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        public async Task<int> productPost(List<ProductViewModel> product, string username)
        {
            int Updated = 1;
            var Ids = product.Select(d => d.Id).ToList();
            var listData = this.DbContext.Products.Where(m => Ids.Contains(m.Id) && !m._IsDeleted).ToList();

            listData.ForEach(async m =>
            {
                Updated = await productUpdated(m, username);

                //m.Active = true;
                //m.FlagForUpdate("dev2", UserAgent);




                //DbContext.Products.Update(m);

                //Updated += await DbContext.SaveChangesAsync();


            });
            
            return Updated;
        }

        public async Task<int> productUpdated(Product model, string username)
        {
            
               
            model.Active = true;
            model.FlagForUpdate(username, UserAgent);




            DbContext.Products.Update(model);
           
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> productNonActive(int Id, string username)
        {
           
            
            var model = this.DbContext.Products.FirstOrDefault( x => x.Id == Id);

            model.Active = false;
            model.FlagForUpdate(username, UserAgent);

            DbContext.Products.Update(model);
            return await DbContext.SaveChangesAsync();
        }
        
        public async Task<int> UpdateProduct(int Id, string username, ProductViewModel product )
        {

            //var modelHistory = new ProductPriceHistoryModel();


            var model = this.DbContext.Products.FirstOrDefault(x => x.Id == Id);
            if (product.IsPriceChange)
            {

                var dataHistory = this.DbContext.ProductPriceHistories.OrderByDescending( s => s._CreatedUtc).FirstOrDefault(a => a.ProductId == Id);

                var modelHistory = new ProductPriceHistoryModel();
                if (dataHistory != null) {
                    modelHistory = new ProductPriceHistoryModel(
                    product.Code, product.Name, model.Price, (decimal)product.Price, product.Currency.Id, product.Currency.Code, product.Currency.Symbol, product.EditReason, product.UOM.Id, product.UOM.Unit, model.Id, dataHistory._CreatedUtc);
                }
                else {
                    modelHistory = new ProductPriceHistoryModel(
                    product.Code, product.Name, model.Price, (decimal)product.Price, product.Currency.Id, product.Currency.Code, product.Currency.Symbol, product.EditReason, product.UOM.Id, product.UOM.Unit, model.Id, model._CreatedUtc);
                }

                

                modelHistory.FlagForCreate(username, UserAgent);
                _dbContext.ProductPriceHistories.Add(modelHistory);
            }

            model.Name = product.Name;
            model.UomId = product.UOM.Id;
            model.UomUnit = product.UOM.Unit;
            model.CurrencyId = product.Currency.Id;
            model.CurrencyCode = product.Currency.Code;
            model.CurrencySymbol = product.Currency.Symbol;
            model.Price = (decimal)product.Price;
            model.Tags = product.Tags;
            model.Description = product.Description;

            model.FlagForUpdate(username, UserAgent);

            DbContext.Products.Update(model);
            return await DbContext.SaveChangesAsync();
        }

        public List<MonitoringProductViewModel> GetReport(int productId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            var Query = GetReportQuery(productId, dateFrom, dateTo).ToList();

            return Query;
        }

        public IQueryable<MonitoringProductViewModel> GetReportQuery(int productId ,  DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            

            var Query = DbContext.ProductPriceHistories.AsNoTracking();

            if (productId != 0) {
                Query = Query.Where(s => s.ProductId == productId);
            }

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                Query = Query.Where(s => dateFrom.Value.Date <= s._CreatedUtc.Date &&
                            s._CreatedUtc.Date <= dateTo.Value.Date);
            }
            var data = Query.Select(s => new MonitoringProductViewModel()
            {
                Code = s.Code,
                Name = s.Name,
                DateIn = s.DateBefore,
                UOMUnit = s.UomUnit,
                CurrencyCode = s.CurrencyCode,
                PriceOrigin = s.PriceOrigin,
                Price = s.Price,
                DiffPrice =  s.Price - s.PriceOrigin,
                DiffPricePercentage = (((s.Price - s.PriceOrigin)/s.PriceOrigin )) * 100,
                EditReason = s.EditReason,
                DateChange = s._CreatedUtc

            }).OrderByDescending(b => b.Code).ThenByDescending(s => s.DateIn);
            return data;
        }

        public MemoryStream GenerateExcel(int productId, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            var Query = GetReportQuery(productId, dateFrom, dateTo);
            Query = Query.OrderByDescending(b => b.Code).ThenByDescending(s => s.DateIn);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Input", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Mata Uang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Lama", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga Baru", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Selisih Harga", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Persentase Selisih ", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Alasan Perubahan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Perubahan", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("","","","","",0,0,0,"",""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string percentage = (item.DiffPricePercentage/100).ToString("P", CultureInfo.InvariantCulture);
                    string inputDate = item.DateIn == new DateTime(1970, 1, 1) || item.DateIn.ToString("dd MMMM yyyy") == "01 Jan 0001" ? "-" : item.DateIn.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
                    string changeDate = item.DateChange == new DateTime(1970, 1, 1) ? "-" : item.DateChange.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(item.Code, item.Name, inputDate, item.UOMUnit, item.CurrencyCode, item.PriceOrigin, item.Price, Math.Round( item.DiffPrice, 2), percentage,
                       item.EditReason, changeDate );
                }
            }

            ExcelPackage package = new ExcelPackage();
            bool styling = true;
            var Data = Query;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);
                sheet.Cells["A1"].LoadFromDataTable(item.Key, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);

                Dictionary<string, int> counts = new Dictionary<string, int>();
                Dictionary<string, int> countsType = new Dictionary<string, int>();
                //var docNo = Data.ToArray();
                int value;

                foreach (var a in Data)
                {
                    if (counts.TryGetValue(a.Code, out value))
                    {
                        counts[a.Code]++;
                    }
                    else
                    {
                        counts[a.Code] = 1;
                    }
                }

                int index = 2;
                foreach (KeyValuePair<string, int> b in counts)
                {
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index + ":A" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["B" + index + ":B" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["C" + index + ":C" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["D" + index + ":D" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Merge = true;
                    sheet.Cells["E" + index + ":E" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["F" + index + ":F" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["G" + index + ":G" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["H" + index + ":H" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["H" + index + ":H" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["I" + index + ":I" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["I" + index + ":I" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["J" + index + ":J" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["J" + index + ":J" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["K" + index + ":K" + (index + b.Value - 1)].Merge = true;
                    //sheet.Cells["K" + index + ":K" + (index + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;



                    index += b.Value;

                    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                }
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;

            //return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
    }
}