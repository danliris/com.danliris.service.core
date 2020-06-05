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
    public class FabricTypeService : BasicService<CoreDbContext, FabricType>, IBasicUploadCsvService<FabricTypeViewModel>, IMap<FabricType,FabricTypeViewModel>
    {
        public FabricTypeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<FabricType>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null,string Filter="{}")
        {
            IQueryable<FabricType> Query = this.DbContext.FabricTypes;
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
                .Select(u => new FabricType
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
            Pageable<FabricType> pageable = new Pageable<FabricType>(Query, Page - 1, Size);
            List<FabricType> Data = pageable.Data.ToList<FabricType>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public FabricTypeViewModel MapToViewModel(FabricType fabrictype)
        {
            FabricTypeViewModel fabrictypeVM = new FabricTypeViewModel
            {
                Id = fabrictype.Id,
                UId = fabrictype.UId,
                _IsDeleted = fabrictype._IsDeleted,
                Active = fabrictype.Active,
                _CreatedUtc = fabrictype._CreatedUtc,
                _CreatedBy = fabrictype._CreatedBy,
                _CreatedAgent = fabrictype._CreatedAgent,
                _LastModifiedUtc = fabrictype._LastModifiedUtc,
                _LastModifiedBy = fabrictype._LastModifiedBy,
                _LastModifiedAgent = fabrictype._LastModifiedAgent,
                Name = fabrictype.Name
            };

            return fabrictypeVM;
        }

        public FabricType MapToModel(FabricTypeViewModel fabrictypeVM)
        {
            FabricType fabrictypey = new FabricType
            {
                Id = fabrictypeVM.Id,
                UId = fabrictypeVM.UId,
                _IsDeleted = fabrictypeVM._IsDeleted,
                Active = fabrictypeVM.Active,
                _CreatedUtc = fabrictypeVM._CreatedUtc,
                _CreatedBy = fabrictypeVM._CreatedBy,
                _CreatedAgent = fabrictypeVM._CreatedAgent,
                _LastModifiedUtc = fabrictypeVM._LastModifiedUtc,
                _LastModifiedBy = fabrictypeVM._LastModifiedBy,
                _LastModifiedAgent = fabrictypeVM._LastModifiedAgent,
                Name = fabrictypeVM.Name
            };

            return fabrictypey;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Name"
        };

        public List<string> CsvHeader => Header;

        public sealed class FabricTypeMap : ClassMap<FabricTypeViewModel>
        {
            public FabricTypeMap()
            {
                Map(u => u.Name).Index(0);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<FabricTypeViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (FabricTypeViewModel fabrictypeVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(fabrictypeVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != fabrictypeVM && d.Name.Equals(fabrictypeVM.Name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh duplikat, ");
                }

                if(string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if(this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(fabrictypeVM.Name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Name tidak boleh duplikat, ");
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMessage)) /* Not Empty */
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Name", fabrictypeVM.Name);
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

        public List<FabricType> GetSimple()
        {
            return this.DbSet.Select(x => new FabricType()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();
        }        
    }
}