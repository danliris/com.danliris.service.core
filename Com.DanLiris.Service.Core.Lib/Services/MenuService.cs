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
using CsvHelper;
using CsvHelper.Configuration;
using System.Dynamic;
using System.IO;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Com.DanLiris.Service.Core.Lib.Models.Account_and_Roles;
using System.Text.RegularExpressions;

namespace Com.DanLiris.Service.Core.Lib.Services
{
    public class MenuService : BasicService<CoreDbContext, Menus>, IBasicUploadCsvService<MenuViewModel>, IMap<Menus, MenuViewModel>
    {
        public MenuService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _cache = serviceProvider.GetService<IDistributedCache>();
        }
        public override Tuple<List<Menus>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
        {
            IQueryable<Menus> Query = this.DbContext.Menus;
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(Filter);
            Query = ConfigureFilter(Query, FilterDictionary);
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);

            /* Search With Keyword */
            if (Keyword != null)
            {
                List<string> SearchAttributes = new List<string>()
                {
                    "Code", "Menu", "SubMenu","MenuName"
                };

                Query = Query.Where(General.BuildSearch(SearchAttributes), Keyword);
            }

            /* Const Select */
            List<string> SelectedFields = new List<string>()
            {
                "Id", "Code",  "Menu", "SubMenu","MenuName"
            };

            Query = Query
                .Select(u => new Menus
                {
                    Id = u.Id,
                    Code = u.Code,
                    Menu = u.Menu,
                    SubMenu = u.SubMenu,
                    MenuName = u.MenuName,
                });

            /* Order */
            if (OrderDictionary.Count.Equals(0))
            {
                OrderDictionary.Add("_updatedDate", General.DESCENDING);

                //Query = Query.OrderByDescending(b => b._LastModifiedUtc); /* Default Order */

                Query = Query.OrderBy(x => PadNumbers(x.Code));

                //Query = Query.OrderBy(str => Regex.Split(str.Code.Replace(" "," "),("([0-9]+)").Select(Convert)));

                //Query.Sort();
                //Query = Query.OrderBy(x => new { x.Menu,x.SubMenu,x });
                //Query = Query.GroupBy(x => new { x.Code, x.Menu, x.SubMenu, x.MenuName }, (key, group) => new Menus
                //{
                //    Code = key.Code,
                //    Menu = key.Menu,
                //    SubMenu = key.SubMenu,
                //    MenuName = key.MenuName
                //});
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
            //Pageable<Menus> pageable = new Pageable<Menus>(Query, Page - 1, Size);

            Pageable<Menus> pageable = new Pageable<Menus>(Query, Page - 1, Size);
            List<Menus> Data = pageable.Data.ToList<Menus>();

            int TotalData = pageable.TotalCount;

            //SetCache();

            return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
        }

        public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }
        public MenuViewModel MapToViewModel(Menus unit)
        {
            MenuViewModel unitVM = new MenuViewModel();

            unitVM.Id = unit.Id;
            unitVM.UId = unit.UId;
            unitVM._IsDeleted = unit._IsDeleted;
            unitVM.Active = unit.Active;
            unitVM._CreatedUtc = unit._CreatedUtc;
            unitVM._CreatedBy = unit._CreatedBy;
            unitVM._CreatedAgent = unit._CreatedAgent;
            unitVM._LastModifiedUtc = unit._LastModifiedUtc;
            unitVM._LastModifiedBy = unit._LastModifiedBy;
            unitVM._LastModifiedAgent = unit._LastModifiedAgent;
            unitVM.Code = unit.Code;
            unitVM.Menu = unit.Menu;
            unitVM.SubMenu = unit.SubMenu;
            unitVM.MenuName = unit.MenuName;
            unitVM.Description = unit.Description;

            return unitVM;
        }
        public Menus MapToModel(MenuViewModel unitVM)
        {
            Menus unit = new Menus();

            unit.Id = unitVM.Id;
            unit.UId = unitVM.UId;
            unit._IsDeleted = unitVM._IsDeleted;
            unit.Active = unitVM.Active;
            unit._CreatedUtc = unitVM._CreatedUtc;
            unit._CreatedBy = unitVM._CreatedBy;
            unit._CreatedAgent = unitVM._CreatedAgent;
            unit._LastModifiedUtc = unitVM._LastModifiedUtc;
            unit._LastModifiedBy = unitVM._LastModifiedBy;
            unit._LastModifiedAgent = unitVM._LastModifiedAgent;
            unit.Code = unitVM.Code;
            unit.Menu = unitVM.Menu;
            unit.SubMenu = unitVM.SubMenu;
            unit.MenuName = unitVM.MenuName;
            unit.Description = unitVM.Description;

            return unit;
        }
        /* Upload CSV */
        private readonly List<string> Header = new List<string>()
        {
            "Kode", "Menu", "Sub Menu", "Nama Menu"
        };
        private readonly IDistributedCache _cache;

        protected override void SetCache()
        {
            var units = DbContext.Menus.ToList();
            _cache.SetString("Menu", JsonConvert.SerializeObject(units));
        }
        public List<string> CsvHeader => Header;

        public MemoryStream DownloadTemplate()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {

                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        foreach (var item in CsvHeader)
                        {
                            csvWriter.WriteField(item);
                        }
                        csvWriter.NextRecord();
                    }
                }
                return stream;
            }
        }

        public sealed class MenuMap : ClassMap<MenuViewModel>
        {
            public MenuMap()
            {
                Map(b => b.Code).Index(0);
                Map(b => b.Menu).Index(1);
                Map(b => b.SubMenu).Index(2);
                Map(b => b.MenuName).Index(3);
            }
        }
        public Tuple<bool, List<object>> UploadValidate(List<MenuViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;

            foreach (MenuViewModel unitVM in Data)
            {
                ErrorMessage = "";

                if (string.IsNullOrWhiteSpace(unitVM.Code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != unitVM && d.Code.Equals(unitVM.Code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Kode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(unitVM.Menu))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Menu tidak boleh kosong, ");
                }


                if (string.IsNullOrWhiteSpace(unitVM.SubMenu))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "SubMenu tidak boleh kosong, ");
                }


                if (string.IsNullOrWhiteSpace(unitVM.MenuName))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "MenuName tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != unitVM && d.MenuName.Equals(unitVM.MenuName)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "MenuName tidak boleh duplikat, ");
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
