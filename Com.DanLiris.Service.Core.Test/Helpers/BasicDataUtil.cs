using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Helpers;
using Com.DanLiris.Service.Core.Lib.Services;
using Com.Moonlay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.Helpers
{
    public abstract class BasicDataUtil<TDbContext, TService, TModel>
        where TDbContext : DbContext
        where TService : BasicService<TDbContext, TModel>
        where TModel : StandardEntity, IValidatableObject
    {
        private CoreDbContext dbContext;
        private BuyerService service;

        public TDbContext DbContext { get; set; }
        public TService Service { get; set; }

        public BasicDataUtil(TDbContext dbContext, TService service)
        {
            DbContext = dbContext;
            Service = service;
            Service.Username = "Unit Test";
        }

        protected BasicDataUtil(CoreDbContext dbContext, BuyerService service)
        {
            this.dbContext = dbContext;
            this.service = service;
        }

        public abstract TModel GetNewData();
        public abstract Task<TModel> GetTestDataAsync();
    }
}
