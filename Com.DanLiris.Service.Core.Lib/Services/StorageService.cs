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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class StorageService : BasicService<CoreDbContext, Storage>, IMap<Storage, StorageViewModel>
    {
        public StorageService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Storage>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Storage> Query = this.DbContext.Storages;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Name", "UnitName"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "_id", "code", "name", "unit"
            };

            Query = Query
                .Select(s => new Storage
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    UnitName = s.UnitName,
                    DivisionName = s.DivisionName
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
            Pageable<Storage> pageable = new Pageable<Storage>(Query, Page - 1, Size);
            List<Storage> Data = pageable.Data.ToList<Storage>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public StorageViewModel MapToViewModel(Storage storage)
        {
            StorageViewModel storageVM = new StorageViewModel();
            storageVM.unit = new StorageUnitViewModel();
            storageVM.unit.division = new DivisionViewModel();

            storageVM._id = storage.Id;
            storageVM.UId = storage.UId;
            storageVM._deleted = storage._IsDeleted;
            storageVM._active = storage.Active;
            storageVM._createdDate = storage._CreatedUtc;
            storageVM._createdBy = storage._CreatedBy;
            storageVM._createAgent = storage._CreatedAgent;
            storageVM._updatedDate = storage._LastModifiedUtc;
            storageVM._updatedBy = storage._LastModifiedBy;
            storageVM._updateAgent = storage._LastModifiedAgent;
            storageVM.code = storage.Code;
            storageVM.name = storage.Name;
            storageVM.description = storage.Description;
            storageVM.unit._id = storage.UnitId;
            storageVM.unit.name = storage.UnitName;
            storageVM.unit.division.Name = storage.DivisionName;

            return storageVM;
        }

        public Storage MapToModel(StorageViewModel storageVM)
        {
            Storage storage = new Storage();

            storage.Id = storageVM._id;
            storage.UId = storageVM.UId;
            storage._IsDeleted = storageVM._deleted;
            storage.Active = storageVM._active;
            storage._CreatedUtc = storageVM._createdDate;
            storage._CreatedBy = storageVM._createdBy;
            storage._CreatedAgent = storageVM._createAgent;
            storage._LastModifiedUtc = storageVM._updatedDate;
            storage._LastModifiedBy = storageVM._updatedBy;
            storage._LastModifiedAgent = storageVM._updateAgent;
            storage.Code = storageVM.code;
            storage.Name = storageVM.name;
            storage.Description = storageVM.description;

            if (!Equals(storageVM.unit, null))
            {
                storage.UnitId = storageVM.unit._id;
                storage.UnitName = storageVM.unit.name;
                storage.DivisionName = storageVM.unit.division.Name;
            }
            else
            {
                storage.UnitId = null;
                storage.UnitName = null;
                storage.DivisionName = null;
            }

            return storage;
        }

        public override void OnCreating(Storage model)
        {
            CodeGenerator codeGenerator = new CodeGenerator();

            do
            {
                model.Code = codeGenerator.GenerateCode();
            }
            while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            base.OnCreating(model);
        }

        public Task<List<StorageByNameViewModel>> GetStorageByName(string keyword, int page, int size)
        {
            var query = DbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(entity => entity.Name.Contains(keyword));
            }

            return query.Skip((page - 1) * size).Take(size).Select(entity => new StorageByNameViewModel()
            {
                Code = entity.Code,
                Id = entity.Id,
                Name = entity.Name,
                Unit = new UnitStorage()
                {
                    Division = new DivisionStorage()
                    {
                        Name = entity.DivisionName
                    },
                    Name = entity.UnitName
                }
            }).ToListAsync();
        }
    }
}