using Com.DanLiris.Service.Core.Lib.Models;
using Com.DanLiris.Service.Core.Lib.Services.BankCashReceiptType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Test.DataUtils
{
    public class BankCashReceiptTypeDataUtil
    {
        private readonly BankCashReceiptTypeService Service;

        public BankCashReceiptTypeDataUtil(BankCashReceiptTypeService service)
        {
            Service = service;
        }

        public BankCashReceiptTypeModel GetNewData()
        {
            Guid guid = Guid.NewGuid();

            BankCashReceiptTypeModel model = new BankCashReceiptTypeModel
            {
                COACode = $"Code{guid}",
                Name = $"Name{guid}",
                COAName = $"COA{guid}",
                COAId = 1,
            };

            return model;
        }

        public async Task<BankCashReceiptTypeModel> GetTestData()
        {
            var data = GetNewData();

            await Service.CreateAsync(data);

            return data;
        }
    }
}

