using Microsoft.WindowsAzure.Storage.Table;

namespace LunchAgentService.Entities
{
    public class User : TableEntity 
    {
        public User()
        {
            
        }

        public User(string name)
        {
            PartitionKey = nameof(User);
            RowKey = name;
        }

        public string Username => RowKey;
        public string Role { get; set; }
        public string Token { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }

    public class UserApi 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}