using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Lib.Services.IBCurrency
{
    public interface IIBCurrencyService
    {
        int Create(FormDto form);
        IndexDto Read(string keyword, int page, int size, string sort = "{}", string filter = "{}");
        IBCurrencyDto Read(int id);
        int Update(IBCurrencyDto ibCurrency, FormDto form);
        int Delete(IBCurrencyDto ibCurrency);

        string GenerateCSVTemplate();
        CSVResult UploadCSV(List<CSVFormDto> data);
        IBCurrencyDto GetSingleByCodeDate(string code, DateTimeOffset date);
    }
}
