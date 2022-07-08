using Com.DanLiris.Service.Core.Lib.Models;
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
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class GarmentDetailCurrencyService : BasicService<CoreDbContext, GarmentDetailCurrency>, IMap<GarmentDetailCurrency, GarmentDetailCurrencyViewModel>
    {
        public GarmentDetailCurrencyService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<GarmentDetailCurrency>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<GarmentDetailCurrency> Query = this.DbContext.GarmentDetailCurrencies;
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "code", "rate", "date"
            };

            Query = Query
                .Select(g => new GarmentDetailCurrency
                {
                    Id = g.Id,
                    Code = g.Code,
                    Rate = g.Rate,
                    Date = g.Date
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
            Pageable<GarmentDetailCurrency> pageable = new Pageable<GarmentDetailCurrency>(Query, Page - 1, Size);
            List<GarmentDetailCurrency> Data = pageable.Data.ToList<GarmentDetailCurrency>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public GarmentDetailCurrencyViewModel MapToViewModel(GarmentDetailCurrency garmentCurrency)
        {
            GarmentDetailCurrencyViewModel garmentCurrencyVM = new GarmentDetailCurrencyViewModel();

            garmentCurrencyVM.Id = garmentCurrency.Id;
            garmentCurrencyVM._IsDeleted = garmentCurrency._IsDeleted;
            garmentCurrencyVM.Active = garmentCurrency.Active;
            garmentCurrencyVM._CreatedUtc = garmentCurrency._CreatedUtc;
            garmentCurrencyVM._CreatedBy = garmentCurrency._CreatedBy;
            garmentCurrencyVM._CreatedAgent = garmentCurrency._CreatedAgent;
            garmentCurrencyVM._LastModifiedUtc = garmentCurrency._LastModifiedUtc;
            garmentCurrencyVM._LastModifiedBy = garmentCurrency._LastModifiedBy;
            garmentCurrencyVM._LastModifiedAgent = garmentCurrency._LastModifiedAgent;
            garmentCurrencyVM.code = garmentCurrency.Code;
            garmentCurrencyVM.date = garmentCurrency.Date.ToLocalTime();
            garmentCurrencyVM.rate = garmentCurrency.Rate;
            garmentCurrencyVM.Code = garmentCurrency.Code;
            garmentCurrencyVM.Date = garmentCurrency.Date;
            garmentCurrencyVM.Rate = garmentCurrency.Rate;

            return garmentCurrencyVM;
        }

        public GarmentDetailCurrency MapToModel(GarmentDetailCurrencyViewModel garmentCurrencyVM)
        {
            GarmentDetailCurrency garmentCurrency = new GarmentDetailCurrency();

            garmentCurrency.Id = garmentCurrencyVM.Id;
            garmentCurrency._IsDeleted = garmentCurrencyVM._IsDeleted;
            garmentCurrency.Active = garmentCurrencyVM.Active;
            garmentCurrency._CreatedUtc = garmentCurrencyVM._CreatedUtc;
            garmentCurrency._CreatedBy = garmentCurrencyVM._CreatedBy;
            garmentCurrency._CreatedAgent = garmentCurrencyVM._CreatedAgent;
            garmentCurrency._LastModifiedUtc = garmentCurrencyVM._LastModifiedUtc;
            garmentCurrency._LastModifiedBy = garmentCurrencyVM._LastModifiedBy;
            garmentCurrency._LastModifiedAgent = garmentCurrencyVM._LastModifiedAgent;
            garmentCurrency.Code = garmentCurrencyVM.code;
            garmentCurrency.Date = garmentCurrencyVM.date.AddHours(7);
            garmentCurrency.Rate = garmentCurrencyVM.rate;
            
            return garmentCurrency;
        }

        public sealed class GarmentDetailCurrencyMap : ClassMap<GarmentDetailCurrencyViewModel>
        {
            public GarmentDetailCurrencyMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.rate).Index(1).TypeConverter<StringConverter>();
            }
        }
     
        public List<GarmentDetailCurrency> GetByIds(List<int> ids)
        {
            return this.DbSet.Where(p => ids.Contains(p.Id) && p._IsDeleted == false)
                .ToList();
        }

        public List<GarmentDetailCurrency> GetByCode(string code)
        {
            return this.DbSet.Where(p => code.Contains(p.Code) && p._IsDeleted == false)
                .ToList();
        }

        public GarmentDetailCurrency GetSingleByCode(string code)
        {
            return DbSet.OrderByDescending(o => o._LastModifiedUtc).FirstOrDefault(f => f.Code == code);
        }

        public GarmentDetailCurrency GetSingleByCodeDate(string code, DateTime date)
        {
            var currency = DbSet.Where(entity => entity.Code == code && entity.Date.Date == date.Date).ToList().OrderBy(o => o.Date).FirstOrDefault();
            if (currency == null)
                return null;
            return DbSet.FirstOrDefault(entity => entity.Id == currency.Id);
        }

        public List<GarmentDetailCurrencyViewModel> GetSingleByCodeDatePEB(List<GarmentDetailCurrencyViewModel> filters)
        {
            List<GarmentDetailCurrencyViewModel> data = new List<GarmentDetailCurrencyViewModel>();
            foreach (var filter in filters)
            {
                var model = DbSet.Where(q => q.Code == filter.code && q.Date.Date == filter.date.Date).OrderBy(o => o.Date).FirstOrDefault();

                if (data.Count(ac => ac.Id == model.Id) == 0)
                {
                    data.Add(MapToViewModel(model));
                }
            }
            return data;
        }

        public GarmentDetailCurrency GetRatePEB(DateTimeOffset date)
        {
            var currency = DbSet.Where(entity => entity.Code == "USD" && entity.Date <= date).ToList().Select(o => new { Diffs = Math.Abs((o.Date.Date - date.DateTime.Date).Days), o.Date, o.Id }).OrderBy(o => o.Diffs).FirstOrDefault();

            return DbSet.FirstOrDefault(entity => entity.Id == currency.Id);
        }

    }
}
