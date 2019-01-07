﻿using Com.Danliris.Service.Core.Mongo.MongoModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Core.Mongo.MongoRepositories
{
    public interface ISupplierMongoRepository
    {
        Task<IEnumerable<SupplierMongo>> GetByBatch(int startingNumber, int numberOfBatch);
    }
}
