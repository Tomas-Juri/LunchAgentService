using System.Collections.Generic;
using LunchAgentService.Entities;

namespace LunchAgentService.Services.UserService
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<string> GetAll();
        string GetById(string id);
        string Create(UserApi user);
        void Update(UserApi user);
        void Delete(string id);
    }
}