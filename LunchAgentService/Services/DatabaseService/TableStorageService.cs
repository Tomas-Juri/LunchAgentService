using System;
using System.Collections.Generic;
using LunchAgentService.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LunchAgentService.Services.DatabaseService
{
    public class TableStorageService : IStorageService
    {
        private CloudTableClient Client { get; }
        private AppSettings Settings { get; }

        public TableStorageService(IOptions<AppSettings> options)
        {
            Settings = options.Value;
            var account = CloudStorageAccount.Parse(Settings.ConnectionString);
            Client = account.CreateCloudTableClient();
        }

        public T Get<T>(string key) where T : TableEntity, new()
        {
            return Table.ExecuteAsync(TableOperation.Retrieve<T>(typeof(T).Name, key)).Result.Result as T;
        }

        public IEnumerable<T> Get<T>() where T : TableEntity, new()
        {
            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, typeof(T).Name);
            var query = new TableQuery<T>().Where(condition);
            return Table.ExecuteQuerySegmentedAsync(query, null).Result.Results;
        }

        public T AddOrUpdate<T>(T entity) where T : TableEntity, new()
        {
            return Table.ExecuteAsync(TableOperation.InsertOrMerge(entity)).Result.Result as T;
        }

        public IEnumerable<T> AddOrUpdate<T>(T[] entities) where T : TableEntity, new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(string id) where T : TableEntity, new()
        {
            var entity = Get<T>(id);
            if (entity != null)
                Table.ExecuteAsync(TableOperation.Delete(entity));
        }

        private CloudTable Table
        {
            get
            {
                var table = Client.GetTableReference(Settings.DatabaseName);
                table.CreateIfNotExistsAsync().Wait();
                return table;
            }
        }
    }
}