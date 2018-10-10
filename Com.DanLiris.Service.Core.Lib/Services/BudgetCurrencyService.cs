using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Interfaces;
using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.ViewModels;
using Com.Moonlay.NetCore.Lib;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Com.DanLiris.Service.Core.Lib.Services
{
	public class BudgetCurrencyService : BasicService<CoreDbContext, BudgetCurrency>, IBasicUploadCsvService<BudgetCurrencyViewModel>, IMap<BudgetCurrency, BudgetCurrencyViewModel>
	{
		public BudgetCurrencyService(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public override Tuple<List<BudgetCurrency>, int, Dictionary<string, string>, List<string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", List<string> Select = null, string Keyword = null, string Filter = "{}")
		{
			IQueryable<BudgetCurrency> Query = this.DbContext.BudgetCurrencies;
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
				"_id", "code", "rate", "date", "remark"
			};

			Query = Query
				.Select(g => new BudgetCurrency
				{
					Id = g.Id,
					Code = g.Code,
					Rate = g.Rate,
					Date = g.Date,
					Remark = g.Remark
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
			Pageable<BudgetCurrency> pageable = new Pageable<BudgetCurrency>(Query, Page - 1, Size);
			List<BudgetCurrency> Data = pageable.Data.ToList<BudgetCurrency>();

			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData, OrderDictionary, SelectedFields);
		}

		/* Upload CSV */
		private readonly List<string> Header = new List<string>()
		{
			"Mata Uang", "Kurs","Keterangan"
		};

		public List<string> CsvHeader => Header;

		public BudgetCurrency MapToModel(BudgetCurrencyViewModel budgetCurrencyVM)
		{
			BudgetCurrency budgetCurrency = new BudgetCurrency();

			budgetCurrency.Id = budgetCurrencyVM._id;
			budgetCurrency._IsDeleted = budgetCurrencyVM._deleted;
			budgetCurrency.Active = budgetCurrencyVM._active;
			budgetCurrency._CreatedUtc = budgetCurrencyVM._createdDate;
			budgetCurrency._CreatedBy = budgetCurrencyVM._createdBy;
			budgetCurrency._CreatedAgent = budgetCurrencyVM._createAgent;
			budgetCurrency._LastModifiedUtc = budgetCurrencyVM._updatedDate;
			budgetCurrency._LastModifiedBy = budgetCurrencyVM._updatedBy;
			budgetCurrency._LastModifiedAgent = budgetCurrencyVM._updateAgent;
			budgetCurrency.Code = budgetCurrencyVM.code;
			budgetCurrency.Date = budgetCurrencyVM.date;
			budgetCurrency.Rate = !Equals(budgetCurrencyVM.rate, null) ? Convert.ToDouble(budgetCurrencyVM.rate) : null; /* Check Null */
			budgetCurrency.Remark = budgetCurrencyVM.remark;

			return budgetCurrency;
		}

		public BudgetCurrencyViewModel MapToViewModel(BudgetCurrency budgetCurrency)
		{
			BudgetCurrencyViewModel budgetCurrencyVM = new BudgetCurrencyViewModel();

			budgetCurrencyVM._id = budgetCurrency.Id;
			budgetCurrencyVM._deleted = budgetCurrency._IsDeleted;
			budgetCurrencyVM._active = budgetCurrency.Active;
			budgetCurrencyVM._createdDate = budgetCurrency._CreatedUtc;
			budgetCurrencyVM._createdBy = budgetCurrency._CreatedBy;
			budgetCurrencyVM._createAgent = budgetCurrency._CreatedAgent;
			budgetCurrencyVM._updatedDate = budgetCurrency._LastModifiedUtc;
			budgetCurrencyVM._updatedBy = budgetCurrency._LastModifiedBy;
			budgetCurrencyVM._updateAgent = budgetCurrency._LastModifiedAgent;
			budgetCurrencyVM.code = budgetCurrency.Code;
			budgetCurrencyVM.date = budgetCurrency.Date.ToLocalTime();
			budgetCurrencyVM.rate = budgetCurrency.Rate;
			budgetCurrencyVM.remark = budgetCurrency.Remark;

			return budgetCurrencyVM;
		}

		public sealed class BudgetCurrencyMap : ClassMap<BudgetCurrencyViewModel>
		{
			public BudgetCurrencyMap()
			{
				Map(c => c.code).Index(0);
				Map(c => c.rate).Index(1).TypeConverter<StringConverter>();
				Map(c => c.remark).Index(2);
			}
		}

		public Tuple<bool, List<object>> UploadValidate(List<BudgetCurrencyViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
		{
			const string DATE_KEYWORD = "date";

			DateTime Date;
			string DateString = Body.SingleOrDefault(s => s.Key.Equals(DATE_KEYWORD)).Value;
			bool ValidDate = DateTime.TryParseExact(DateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);

			List<object> ErrorList = new List<object>();
			string ErrorMessage;
			bool Valid = true;

			foreach (BudgetCurrencyViewModel budgetCurrencyVM in Data)
			{
				ErrorMessage = "";

				if (string.IsNullOrWhiteSpace(budgetCurrencyVM.code))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak boleh kosong, ");
				}

				double Rate = 0;
				if (string.IsNullOrWhiteSpace(budgetCurrencyVM.rate))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kurs tidak boleh kosong, ");
				}
				else if (!double.TryParse(budgetCurrencyVM.rate, out Rate))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kurs harus numerik, ");
				}
				else if (Rate < 0)
				{
					ErrorMessage = string.Concat(ErrorMessage, "Kurs harus lebih besar dari 0, ");
				}
				else
				{
					string[] RateSplit = budgetCurrencyVM.rate.Split('.');
					if (RateSplit.Count().Equals(2) && RateSplit[1].Length > 4)
					{
						ErrorMessage = string.Concat(ErrorMessage, "Kurs maksimal memiliki 4 digit dibelakang koma, ");
					}
				}

				if (!ValidDate)
				{
					ErrorMessage = string.Concat(ErrorMessage, "Tanggal tidak boleh kosong, ");
				}
				else if (Date > DateTime.Now)
				{
					ErrorMessage = string.Concat(ErrorMessage, "Tanggal tidak boleh lebih dari tanggal hari ini, ");
				}

				if (string.IsNullOrEmpty(ErrorMessage))
				{
					/* Service Validation */
					if (!this.DbContext.Set<Currency>().Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(budgetCurrencyVM.code)))
					{
						ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak terdaftar dalam master Mata Uang, ");
					}

					if (Data.Any(d => d != budgetCurrencyVM && d.code.Equals(budgetCurrencyVM.code) && d.date.Equals(budgetCurrencyVM.date)) || this.DbSet.Any(d => d._IsDeleted.Equals(false) && d.Code.Equals(budgetCurrencyVM.code) && d.Date.Equals(Date)))
					{
						ErrorMessage = string.Concat(ErrorMessage, "Mata Uang dan Tanggal tidak boleh duplikat, ");
					}
				}

				if (string.IsNullOrEmpty(ErrorMessage))
				{
					budgetCurrencyVM.rate = Rate;
					budgetCurrencyVM.date = Date.ToUniversalTime();
				}
				else
				{
					ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
					var Error = new ExpandoObject() as IDictionary<string, object>;

					Error.Add("Mata Uang", budgetCurrencyVM.code);
					Error.Add("Kurs", budgetCurrencyVM.rate);
					Error.Add("Error", ErrorMessage);

					ErrorList.Add(Error);
				}
			}

			if (ErrorList.Count > 0)
			{
				Valid = false;
			}

			return Tuple.Create(Valid, ErrorList); ;
		}

		public List<BudgetCurrency> GetByIds(List<int> ids)
		{
			return this.DbSet.Where(p => ids.Contains(p.Id) && p._IsDeleted == false)
				.ToList();
		}

	}
}
