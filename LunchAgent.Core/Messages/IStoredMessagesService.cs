namespace LunchAgent.Core.Messages;

public interface IStoredMessagesService
{
    Task Store(string spaceName, DateTime day, string messageName);
    
    Task<string?> Get(string spaceName, DateTime day);
}