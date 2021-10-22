using Com.DanLiris.Service.Core.Lib.Helpers.IdentityService;
using Com.DanLiris.Service.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Com.Moonlay.Models;
using Com.DanLiris.Service.Core.Lib.Helpers;
using System.Linq;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using System.Linq.Expressions;

namespace Com.DanLiris.Service.Core.Lib.Services.BankCashReceiptType
{
    public class BankCashReceiptTypeService : IBankCashReceiptTypeService
    {
        private const string _UserAgent = "core-service";
        protected DbSet<BankCashReceiptTypeModel> _DbSet;
        protected IIdentityService _IdentityService;
        public CoreDbContext _DbContext;

        public BankCashReceiptTypeService(IServiceProvider serviceProvider, CoreDbContext dbContext)
        {
            _DbContext = dbContext;
            _DbSet = dbContext.Set<BankCashReceiptTypeModel>();
            _IdentityService = serviceProvider.GetService<IIdentityService>();
        }

        public async Task<int> CreateAsync(BankCashReceiptTypeModel model)
        {
            model.FlagForCreate(_IdentityService.Username, _UserAgent);
            model._LastModifiedAgent = model._CreatedAgent;
            model._LastModifiedBy = model._CreatedBy;
            model._LastModifiedUtc = model._CreatedUtc;

            _DbSet.Add(model);
            return await _DbContext.SaveChangesAsync();
        }

        public ReadResponse<BankCashReceiptTypeModel> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<BankCashReceiptTypeModel> Query = _DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "COACode", "Name", "COAName"
            };
            Query = QueryHelper<BankCashReceiptTypeModel>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<BankCashReceiptTypeModel>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<BankCashReceiptTypeModel>.Order(Query, OrderDictionary);

            Query = Query.Select(s => new BankCashReceiptTypeModel
            {
                Id = s.Id,
                COACode = s.COACode,
                Name = s.Name,
                COAId = s.COAId,
                COAName = s.COAName,
            });

            Pageable<BankCashReceiptTypeModel> pageable = new Pageable<BankCashReceiptTypeModel>(Query, page - 1, size);
            List<BankCashReceiptTypeModel> Data = pageable.Data.ToList();

            int TotalData = pageable.TotalCount;
            return new ReadResponse<BankCashReceiptTypeModel>(Data, TotalData, OrderDictionary, new List<string>());
        }

        public async Task<BankCashReceiptTypeModel> ReadByIdAsync(int id)
        {
            return await _DbSet.Where(w => w.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(int id, BankCashReceiptTypeModel model)
        {
            model.FlagForUpdate(_IdentityService.Username, _UserAgent);
            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var model = _DbSet.Where(w => w.Id == id).FirstOrDefault();
            model.FlagForDelete(_IdentityService.Username, _UserAgent);
            _DbSet.Update(model);
            return await _DbContext.SaveChangesAsync();
        }

        public bool CheckExisting(Expression<Func<BankCashReceiptTypeModel, bool>> filter)
        {
            var count = _DbSet.Where(filter).Count();

            return count > 0;
        }
    }
}
