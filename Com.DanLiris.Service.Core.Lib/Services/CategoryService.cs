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
using Microsoft.Extensions.Primitives;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class CategoryService : BasicService<CoreDbContext, Category>, IBasicUploadCsvService<CategoryViewModel>, IMap<Category, CategoryViewModel>
    {
        public CategoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Tuple<List<Category>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Category> Query = this.DbContext.Categories;
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
                "_id", "code", "name"
            };

            Query = Query
                .Select(b => new Category
                {
                    Id = b.Id,
                    Code = b.Code,
                    Name = b.Name
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
            Pageable<Category> pageable = new Pageable<Category>(Query, Page - 1, Size);
            List<Category> Data = pageable.Data.ToList<Category>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public CategoryViewModel MapToViewModel(Category category)
        {
            CategoryViewModel categoryVM = new CategoryViewModel();

            categoryVM._id = category.Id;
            categoryVM.UId = category.UId;
            categoryVM._deleted = category._IsDeleted;
            categoryVM._active = category.Active;
            categoryVM._createdDate = category._CreatedUtc;
            categoryVM._createdBy = category._CreatedBy;
            categoryVM._createAgent = category._CreatedAgent;
            categoryVM._updatedDate = category._LastModifiedUtc;
            categoryVM._updatedBy = category._LastModifiedBy;
            categoryVM._updateAgent = category._LastModifiedAgent;
            categoryVM.code = category.Code;
            categoryVM.name = category.Name;
            categoryVM.codeRequirement = category.CodeRequirement;

            return categoryVM;
        }

        public Category MapToModel(CategoryViewModel categoryVM)
        {
            Category category = new Category();

            category.Id = categoryVM._id;
            category.UId = categoryVM.UId;
            category._IsDeleted = categoryVM._deleted;
            category.Active = categoryVM._active;
            category._CreatedUtc = categoryVM._createdDate;
            category._CreatedBy = categoryVM._createdBy;
            category._CreatedAgent = categoryVM._createAgent;
            category._LastModifiedUtc = categoryVM._updatedDate;
            category._LastModifiedBy = categoryVM._updatedBy;
            category._LastModifiedAgent = categoryVM._updateAgent;
            category.Code = categoryVM.code;
            category.Name = categoryVM.name;
            category.CodeRequirement = categoryVM.codeRequirement;

            return category;
        }

        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Nama", "Kode Kebutuhan"
        };

        public List<string> CsvHeader => Header;

        public sealed class CategoryMap : ClassMap<CategoryViewModel>
        {
            public CategoryMap()
            {
                Map(c => c.code).Index(0);
                Map(c => c.name).Index(1);
                Map(c => c.codeRequirement).Index(2);
            }
        }

        public Tuple<bool, List<object>> UploadValidate(List<CategoryViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (CategoryViewModel categoryVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(categoryVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != categoryVM && d.code.Equals(categoryVM.code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(categoryVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != categoryVM && d.name.Equals(categoryVM.name)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    /* Service Validation */
                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(categoryVM.code)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                    }

                    if (this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Name.Equals(categoryVM.name)))
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                    }
                }

                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("Kode", categoryVM.code);
                    Error.Add("Nama", categoryVM.name);
                    Error.Add("Kode Kebutuhan", categoryVM.codeRequirement);
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