using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{
    public interface ISupplierMigrationService
    {
        Task<int> RunAsync(int startingNumber, int numberOfBatch);
    }
}
