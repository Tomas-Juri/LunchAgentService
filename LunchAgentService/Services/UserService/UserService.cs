using System;
using System.Collections.Generic;
using System.Linq;
using LunchAgentService.Entities;
using LunchAgentService.Services.DatabaseService;

namespace LunchAgentService.Services.UserService
{
    public class UserService : IUserService
    {
        private IStorageService StorageService { get; }

        public UserService(IStorageService storageService)
        {
            StorageService = storageService;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = StorageService.Get<User>().Where(x => x.Username == username).ToList().FirstOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<string> GetAll()
        {
            return StorageService.Get<User>().Select(x => x.Username);
        }

        public string GetById(string id)
        {
            return StorageService.Get<User>(id).Username;
        }

        public string Create(UserApi user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Password is required");

            var userServer = StorageService.Get<User>().FirstOrDefault(x => x.Username == user.Username);

            if (userServer != null)
                throw new Exception("Username \"" + user.Username + "\" is already taken");

            CreatePasswordHash(user.Password, out var passwordHash, out var passwordSalt);

            userServer = new User(user.Username)
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.User,
            };

            StorageService.AddOrUpdate(userServer);

            return user.Username;
        }

        public void Update(UserApi user)
        {
            var userServer = StorageService.Get<User>(user.Username);

            if (userServer == null)
                throw new Exception("User not found");

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                CreatePasswordHash(user.Password, out var passwordHash, out var passwordSalt);

                userServer.PasswordHash = passwordHash;
                userServer.PasswordSalt = passwordSalt;
            }

            StorageService.AddOrUpdate(userServer);
        }

        public void Delete(string id)
        {
            StorageService.Delete<User>(id);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}