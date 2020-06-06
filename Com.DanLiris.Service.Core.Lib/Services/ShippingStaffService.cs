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
using Com.DanLiris.Service.Core.Lib.Interfaces;
using CsvHelper.Configuration;
using System.Dynamic;
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class ShippingStaffService : BasicService<CoreDbContext, ShippingStaff>, IBasicUploadCsvService<ShippingStaffViewModel>, IMap<ShippingStaff, ShippingStaffViewModel>
    {
        public ShippingStaffService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<ShippingStaff>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<ShippingStaff> Query = this.DbContext.GarmentShippingStaffs;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Name"
            };

            Query = Query
                .Select(u => new ShippingStaff
                {
                    Id = u.Id,
                    Name = u.Name,
                    _LastModifiedUtc = u._LastModifiedUtc
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
            Pageable<ShippingStaff> pageable = new Pageable<ShippingStaff>(Query, Page - 1, Size);
            List<ShippingStaff> Data = pageable.Data.ToList<ShippingStaff>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public ShippingStaffViewModel MapToViewModel(ShippingStaff shippingstaff)
        {
            ShippingStaffViewModel shippingstaffVM = new ShippingStaffViewModel
            {
                Id = shippingstaff.Id,
                UId = shippingstaff.UId,
                _IsDeleted = shippingstaff._IsDeleted,
                Active = shippingstaff.Active,
                _CreatedUtc = shippingstaff._CreatedUtc,
                _CreatedBy = shippingstaff._CreatedBy,
                _CreatedAgent = shippingstaff._CreatedAgent,
                _LastModifiedUtc = shippingstaff._LastModifiedUtc,
                _LastModifiedBy = shippingstaff._LastModifiedBy,
                _LastModifiedAgent = shippingstaff._LastModifiedAgent,
                Name = shippingstaff.Name
            };

            return shippingstaffVM;
        }

        public ShippingStaff MapToModel(ShippingStaffViewModel shippingstaffVM)
        {
            ShippingStaff shippingstaff = new ShippingStaff
            {
                Id = shippingstaffVM.Id,
                UId = shippingstaffVM.UId,
                _IsDeleted = shippingstaffVM._IsDeleted,
                Active = shippingstaffVM.Active,
                _CreatedUtc = shippingstaffVM._CreatedUtc,
                _CreatedBy = shippingstaffVM._CreatedBy,
                _CreatedAgent = shippingstaffVM._CreatedAgent,
                _LastModifiedUtc = shippingstaffVM._LastModifiedUtc,
                _LastModifiedBy = shippingstaffVM._LastModifiedBy,
                _LastModifiedAgent = shippingstaffVM._LastModifiedAgent,
                Name = shippingstaffVM.Name
            };

            return shippingstaff;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Name"
        };

        public List<string> CsvHeader => Header;

        public sealed class ShippingStaffMap : ClassMap<ShippingStaffViewModel>
        {
            public ShippingStaffMap()
            {
                Map(u => u.Name).Index(0);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<ShippingStaffViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (ShippingStaffViewModel shippingstaffVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(shippingstaffVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != shippingstaffVM && d.Name.Equals(shippingstaffVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh duplikat, ");
                }

                if(string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(shippingstaffVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh duplikat, ");
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMessage)) /* Not Empty */
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Name", shippingstaffVM.Name);
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

        public List<ShippingStaff> GetSimple()
        {
            return this.DbSet.Select(x => new ShippingStaff()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();
        }        
    }
}