using Google.Apis.HangoutsChat.v1.Data;

namespace LunchAgent.Core.Google;

public interface IGoogleChatService
{
    Task<IReadOnlyCollection<Space>> GetSpaces();
    
    Task<Message> CreateMessage(Message message, string spaceName);

    Task<Message> UpdateMessage(Message message, string spaceName);
}