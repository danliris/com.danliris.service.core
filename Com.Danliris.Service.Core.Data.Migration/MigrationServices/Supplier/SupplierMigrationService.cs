using Com.Danliris.Service.Core.Mongo.MongoModels;
using Com.Danliris.Service.Core.Mongo.MongoRepositories;
using Com.DanLiris.Service.Core.Lib;
using Com.DanLiris.Service.Core.Lib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Data.Migration.MigrationServices
{ 
    public class SupplierMigrationService : ISupplierMigrationService
    {
        private readonly ISupplierMongoRepository _supplierRepository;
        private readonly CoreDbContext _dbContext;
        private readonly DbSet<Supplier> _supplierDbSet;

        private int TotalInsertedData { get; set; } = 0;

        public SupplierMigrationService(ISupplierMongoRepository supplierRepository, CoreDbContext dbContext)
        {
            _supplierRepository = supplierRepository;
            _dbContext = dbContext;
            _supplierDbSet = _dbContext.Set<Supplier>();
        }

        //public Task<int> RunAsync(int startingNumber, int numberOfBatch)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<int> RunAsync(int startingNumber, int numberOfBatch)
        {
            //Extract from Mongo
            var extractedData = await _supplierRepository.GetByBatch(startingNumber, numberOfBatch);

            if (extractedData.Count() > 0)
            {
                var transformedData = Transform(extractedData);
                startingNumber += transformedData.Count;

                //Insert into SQL
                TotalInsertedData += Load(transformedData);

                await RunAsync(startingNumber, numberOfBatch);
            }

            return TotalInsertedData;
        }

        private List<Supplier> Transform(IEnumerable<SupplierMongo> extractedData)
        {
            return extractedData.Select(mongoSupplier => new Supplier(mongoSupplier)).ToList();
        }

        private int Load(List<Supplier> transformedData)
        {
            var existingUids = _supplierDbSet.Select(entity => entity.UId).ToList();
            transformedData = transformedData.Where(entity => !existingUids.Contains(entity.UId)).ToList();
            if (transformedData.Count > 0)
                _supplierDbSet.AddRange(transformedData);
            return _dbContext.SaveChanges();
        }
    }
}