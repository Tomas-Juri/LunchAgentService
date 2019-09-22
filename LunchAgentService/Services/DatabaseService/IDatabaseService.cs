using System;
using System.Collections.Generic;
using LunchAgentService.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LunchAgentService.Services.DatabaseService
{
    public interface IDatabaseService
    {
        T Get<T>(ObjectId id) where T : MongoEntity;
        IEnumerable<T> Get<T>() where T : MongoEntity;
        ReplaceOneResult AddOrUpdate<T>(T entity) where T : MongoEntity;
        IEnumerable<T> AddOrUpdate<T>(T[] entities) where T : MongoEntity;
        DeleteResult Delete<T>(ObjectId id) where T : MongoEntity;
    }

}