using System.Globalization;
using System.Text.RegularExpressions;
using Azure;
using Azure.Data.Tables;

namespace LunchAgent.Core.Messages;

public class StoredMessagesService : IStoredMessagesService
{
    private readonly TableClient _tableClient;

    public StoredMessagesService(string connectionString)
    {
        var tableServiceClient = new TableServiceClient(connectionString);
        _tableClient = tableServiceClient.GetTableClient("messages");
    }

    public async Task Store(string spaceName, DateTime day, string messageName)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var entity = new StoredMessageEntity
        {
            PartitionKey = GetPartitionKey(spaceName),
            RowKey = GetRowKey(day),
            MessageName = messageName
        };

        await _tableClient.UpsertEntityAsync(entity);
    }

    public async Task<string?> Get(string spaceName, DateTime day)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var entity =
            await _tableClient.GetEntityIfExistsAsync<StoredMessageEntity>(GetPartitionKey(spaceName), GetRowKey(day));

        return entity.HasValue ? entity.Value.MessageName : null;
    }

    private static string GetPartitionKey(string spaceName)
        => spaceName.Replace(@"spaces/", "");

    private static string GetRowKey(DateTime day)
        => $"{day.Day}{day.Month}{day.Year}";

    public class StoredMessageEntity : ITableEntity
    {
        /// <summary>
        /// Space Name
        /// </summary>
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// DateTime
        /// </summary>
        public string RowKey { get; set; } = string.Empty;

        public string MessageName { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}