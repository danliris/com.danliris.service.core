using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class TrackService : BasicService<CoreDbContext, TrackModel>, IBasicUploadCsvService<TrackViewModel>, IMap<TrackModel, TrackViewModel>
    {
        public TrackService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            DbContext.Database.SetCommandTimeout(1000 * 60 * 2);
        }
        public override Tuple<List<TrackModel>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<TrackModel> Query = this.DbContext.Track.AsNoTracking();
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Type", "Name"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Type", "Name",
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

            List<TrackModel> Data = Query.ToList();

            //int TotalData = Query.TotalCount;

            return Tuple.Create(Data, totalData, OrderDictionary, SelectedFields);
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "Tipe", "Nama", "Box"
        };

        public sealed class TrackMap : ClassMap<TrackViewModel>
        {
            public TrackMap()
            {
                Map(p => p.Type).Index(0);
                Map(p => p.Name).Index(1);
                Map(p => p.Box).Index(2);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<TrackViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            foreach (TrackViewModel TrackVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(TrackVM.Type))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Tipe tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(TrackVM.Box))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Box tidak boleh kosong, ");
                }


                if (string.IsNullOrWhiteSpace(TrackVM.Name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                //else if (Data.Any(d => d != TrackVM && d.Name.Equals(TrackVM.Name)))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                //}
                //else if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(TrackVM.Name)))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Nama Jalur/Rak Sudak Di Input, ");
               // }

                if (Data.Any(d => d != TrackVM /*&& d.Name.Equals(TrackVM.Name)*/))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                //if (Data.Count(d => d != TrackVM && d.Name.Equals(TrackVM.Name) && d.Box.Equals(TrackVM.Box) && d.Type.Equals(TrackVM.Type)) > 0)
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Jalur/Track tidak boleh duplikat, ");
                //}
                if (this.DbSet.Count(d => d._IsDeleted.Equals(false) && d.Name.Equals(TrackVM.Name) && d.Type.Equals(TrackVM.Type) && d.Box.Equals(TrackVM.Box)) > 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama Jalur/Rak Sudak Di Input, ");
                }
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode Barang", TrackVM.Type);
                    Error.Add("Nama Barang", TrackVM.Name);
                    Error.Add("Box", TrackVM.Box);
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

        public TrackModel MapToModel(TrackViewModel trackVM)
        {
            TrackModel trackT = new TrackModel
            {
                Id = trackVM.Id,
                _IsDeleted = trackVM._IsDeleted,
                Active = trackVM.Active,
                _CreatedUtc = trackVM._CreatedUtc,
                _CreatedBy = trackVM._CreatedBy,
                _CreatedAgent = trackVM._CreatedAgent,
                _LastModifiedUtc = trackVM._LastModifiedUtc,
                _LastModifiedBy = trackVM._LastModifiedBy,
                _LastModifiedAgent = trackVM._LastModifiedAgent,
                Type = trackVM.Type,
                Name = trackVM.Name,
                Box = trackVM.Box,
            };


            return trackT;
        }

        public TrackViewModel MapToViewModel(TrackModel trackT)
        {
            TrackViewModel trackVM = new TrackViewModel
            {
                Id = trackT.Id,
                _IsDeleted = trackT._IsDeleted,
                Active = trackT.Active,
                _CreatedUtc = trackT._CreatedUtc,
                _CreatedBy = trackT._CreatedBy,
                _CreatedAgent = trackT._CreatedAgent,
                _LastModifiedUtc = trackT._LastModifiedUtc,
                _LastModifiedBy = trackT._LastModifiedBy,
                _LastModifiedAgent = trackT._LastModifiedAgent,
                Type = trackT.Type,
                Name = trackT.Name,
                Box = trackT.Box,


            };

            return trackVM;
        }
    }
}
