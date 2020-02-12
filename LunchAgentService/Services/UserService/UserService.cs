using System;
using System.Collections.Generic;
using System.Linq;
using LunchAgentService.Entities;
using LunchAgentService.Services.DatabaseService;
using MongoDB.Bson;

namespace LunchAgentService.Services.UserService
{
    public class UserService : IUserService
    {
        private IDatabaseService DatabaseService { get; }

        public UserService(IDatabaseService databaseService)
        {
            DatabaseService = databaseService;
        }

        public UserMongo Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = DatabaseService.Get<UserMongo>().Where(x => x.Username == username).ToList().FirstOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<UserApi> GetAll()
        {
            return DatabaseService.Get<UserMongo>().Select(x => x.ToApi());
        }

        public UserApi GetById(string id)
        {
            return DatabaseService.Get<UserMongo>(ObjectId.Parse(id)).ToApi();
        }

        public UserApi Create(UserApi user)
        {
            // validation
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new Exception("Password is required");

            var mongoUser = DatabaseService.Get<UserMongo>().FirstOrDefault(x => x.Username == user.Username);

            if (mongoUser != null)
                throw new Exception("Username \"" + user.Username + "\" is already taken");

            CreatePasswordHash(user.Password, out var passwordHash, out var passwordSalt);

            mongoUser = new UserMongo
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.User,
                Username = user.Username,
            };

            DatabaseService.AddOrUpdate(mongoUser);

            return user;
        }

        public void Update(UserApi user)
        {
            var mongoUser = DatabaseService.Get<UserMongo>(ObjectId.Parse(user.Id));

            if (mongoUser == null)
                throw new Exception("User not found");

            if (user.Username != mongoUser.Username)
            {
                if (DatabaseService.Get<UserMongo>().Any(x => x.Username == user.Username))
                    throw new Exception("Username " + user.Username + " is already taken");
            }

            // update user properties
            mongoUser.Username = user.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                CreatePasswordHash(user.Password, out var passwordHash, out var passwordSalt);

                mongoUser.PasswordHash = passwordHash;
                mongoUser.PasswordSalt = passwordSalt;
            }

            DatabaseService.AddOrUpdate(mongoUser);
        }

        public void Delete(string id)
        {
            DatabaseService.Delete<UserMongo>(ObjectId.Parse(id));
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