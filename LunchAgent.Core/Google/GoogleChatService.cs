using Google.Apis.Auth.OAuth2;
using Google.Apis.HangoutsChat.v1;
using Google.Apis.HangoutsChat.v1.Data;
using Google.Apis.Services;

namespace LunchAgent.Core.Google;

public class GoogleChatService : IGoogleChatService
{
    private readonly HangoutsChatService _chatService;

    public GoogleChatService(string creds)
    {
        var scopes = new[] { "https://www.googleapis.com/auth/chat.bot" };

        var credential = GoogleCredential.FromJson(creds).CreateScoped(scopes);
        _chatService = new HangoutsChatService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyApplication"
        });
    }
    
    public async Task<IReadOnlyCollection<Space>> GetSpaces()
    {
        return (await _chatService.Spaces.List().ExecuteAsync()).Spaces.ToList();
    }

    public async Task<Message> CreateMessage(Message message, string spaceName)
    {
        return await _chatService.Spaces.Messages.Create(message, spaceName).ExecuteAsync();
    }

    public async Task<Message> UpdateMessage(Message message, string messageName)
    {
        var request = _chatService.Spaces.Messages.Update(message, messageName);
        request.UpdateMask = "text";
        
        return await request.ExecuteAsync();
    }
}