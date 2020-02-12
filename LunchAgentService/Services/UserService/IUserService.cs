using System.Collections.Generic;
using LunchAgentService.Entities;

namespace LunchAgentService.Services.UserService
{
    public interface IUserService
    {
        UserMongo Authenticate(string username, string password);
        IEnumerable<UserApi> GetAll();
        UserApi GetById(string id);
        UserApi Create(UserApi user);
        void Update(UserApi user);
        void Delete(string id);
    }
}