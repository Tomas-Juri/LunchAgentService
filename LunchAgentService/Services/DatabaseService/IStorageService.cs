using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace LunchAgentService.Services.DatabaseService
{
    public interface IStorageService
    {
        T Get<T>(string id) where T : TableEntity, new();
        IEnumerable<T> Get<T>() where T : TableEntity, new();
        T AddOrUpdate<T>(T entity) where T : TableEntity, new();
        IEnumerable<T> AddOrUpdate<T>(T[] entities) where T : TableEntity, new();
        void Delete<T>(string id) where T : TableEntity, new();
    }
}