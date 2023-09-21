using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using System.Linq.Dynamic.Core;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Com.Moonlay.NetCore.Lib;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class DebitCreditNoteService : BasicService<CoreDbContext, DebitCreditNoteModel>,  IMap<DebitCreditNoteModel, DebitCreditNoteViewModel>
    {
        public DebitCreditNoteService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public override Tuple<List<DebitCreditNoteModel>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<DebitCreditNoteModel> Query = this.DbContext.DebitCreditNotes;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "TypeDCN","ItemTypeDCN",
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "TypeDCN","ItemTypeDCN",
            };

            Query = Query
                .Select(u => new DebitCreditNoteModel
                {
                    Id = u.Id,
                    TypeDCN = u.TypeDCN,
                    ItemTypeDCN = u.ItemTypeDCN,
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
            Pageable<DebitCreditNoteModel> pageable = new Pageable<DebitCreditNoteModel>(Query, Page - 1, Size);
            List<DebitCreditNoteModel> Data = pageable.Data.ToList<DebitCreditNoteModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public DebitCreditNoteViewModel MapToViewModel(DebitCreditNoteModel dcn)
        {
            DebitCreditNoteViewModel dcnVM = new DebitCreditNoteViewModel
            {
                Id = dcn.Id,

                _IsDeleted = dcn._IsDeleted,
                Active = dcn.Active,
                _CreatedUtc = dcn._CreatedUtc,
                _CreatedBy = dcn._CreatedBy,
                _CreatedAgent = dcn._CreatedAgent,
                _LastModifiedUtc = dcn._LastModifiedUtc,
                _LastModifiedBy = dcn._LastModifiedBy,
                _LastModifiedAgent = dcn._LastModifiedAgent,
                TypeDCN = dcn.TypeDCN,
                ItemTypeDCN = dcn.ItemTypeDCN
            };

            return dcnVM;
        }

        public DebitCreditNoteModel MapToModel(DebitCreditNoteViewModel dcnVM)
        {
            DebitCreditNoteModel dcn = new DebitCreditNoteModel
            {
                Id = dcnVM.Id,
                _IsDeleted = dcnVM._IsDeleted,
                Active = dcnVM.Active,
                _CreatedUtc = dcnVM._CreatedUtc,
                _CreatedBy = dcnVM._CreatedBy,
                _CreatedAgent = dcnVM._CreatedAgent,
                _LastModifiedUtc = dcnVM._LastModifiedUtc,
                _LastModifiedBy = dcnVM._LastModifiedBy,
                _LastModifiedAgent = dcnVM._LastModifiedAgent,
                TypeDCN = dcnVM.TypeDCN,
                ItemTypeDCN = dcnVM.ItemTypeDCN
            };

            return dcn;
        }

    }
}
