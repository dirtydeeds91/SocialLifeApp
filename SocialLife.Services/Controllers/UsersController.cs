using SocialLife.Data;
using SocialLife.Models;
using SocialLife.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SocialLife.Services.Controllers
{
    public class UsersController : ApiController
    {
        private IDbContextFactory<DbContext> contextFactory;

        private static Random rand = new Random();

        private const int AuthCodeLength = 40;
        private const int SessionKeyLength = 50;
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private const string ValidUsernameChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890.";
        private const string ValidDisplayNameChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890._ -";
        private const int MinUserLengthString = 6;
        private const int MaxUserLengthString = 30;

        public UsersController()
        {
            this.contextFactory = new SocialLifeDbContextFactory();
        }

        public UsersController(IDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserModel user)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                this.ValidateUsername(user.Username);
                this.ValidateAuthCode(user.AuthCode);
                this.ValidateDisplayName(user.DisplayName);
                var userToAdd = context.Users.Where(us => us.Username == user.Username).FirstOrDefault();

                if (userToAdd == null)
                {
                    userToAdd = new User()
                    {
                        Username = user.Username,
                        DisplayName = user.DisplayName,
                        AuthCode = user.AuthCode
                    };

                    context.Users.Add(userToAdd);
                    context.SaveChanges();

                    var userProfile = new Profile()
                    {
                        User = userToAdd,
                        UserId = userToAdd.UserId,
                    };

                    userProfile.Status = context.Statuses.Where(st => st.StatusId == 1).FirstOrDefault();

                    context.Profiles.Add(userProfile);
                    context.SaveChanges();

                    string sessionKey = userToAdd.SessionKey;

                    if (sessionKey == null)
                    {
                        sessionKey = CreateSessionKey(userToAdd.UserId);
                        userToAdd.SessionKey = sessionKey;
                        context.SaveChanges();
                    }

                    var registeredUser = new UserModel()
                    {
                        Username = user.Username,
                        DisplayName = user.DisplayName,
                        SessionKey = sessionKey,
                        Id = userToAdd.UserId
                    };

                    var response = Request.CreateResponse<UserModel>(HttpStatusCode.OK, registeredUser); //FIX THIS
                    return response;
                }
                else
                {
                    throw new ArgumentException("This user already exists.");
                }
            }
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserModel user)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.Username == user.Username
                                                                        && us.AuthCode == user.AuthCode).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("Incorrect user or password.");
                }

                string sessionKey = userEntity.SessionKey;

                if (userEntity.SessionKey == null)
                {
                    sessionKey = this.CreateSessionKey(userEntity.UserId);
                    userEntity.SessionKey = sessionKey;
                    context.SaveChanges();
                }

                UserModel loggedUser = new UserModel()
                {
                    Username = userEntity.Username,
                    SessionKey = sessionKey
                };

                var response = Request.CreateResponse<UserModel>(HttpStatusCode.OK, loggedUser);
                return response;
            }
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage Logout(string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                userEntity.SessionKey = null;
                context.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username cannot be empty.");
            }
            else if (username.Length < MinUserLengthString && username.Length > MaxUserLengthString)
            {
                throw new ArgumentOutOfRangeException("Username must be between 6 and 30 symbols.");
            }
            else if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException("Valid characters for username are letters, numbers and dot (.)");
            }
        }

        private void ValidateDisplayName(string displayName)
        {
            if (displayName == null)
            {
                throw new ArgumentNullException("Username cannot be empty.");
            }
            else if (displayName.Length < MinUserLengthString && displayName.Length > MaxUserLengthString)
            {
                throw new ArgumentOutOfRangeException("Username must be between 6 and 30 symbols.");
            }
            else if (displayName.Any(ch => !ValidDisplayNameChars.Contains(ch)))
            {
                throw new ArgumentOutOfRangeException("Valid characters for display name are letters, numbers and dot (.)");
            }
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode.Length != AuthCodeLength)
            {
                throw new ArgumentOutOfRangeException("Invalid authentication code length", "INV_USR_AUTH_LEN");
            }
        }

        private static void ValidateSessionKey(string sessionKey)
        {
            if (sessionKey.Length != SessionKeyLength || sessionKey.Any(ch => !SessionKeyChars.Contains(ch)))
            {
                throw new ArgumentException("Invalid Password", "ERR_INV_AUTH");
            }
        }

        private string CreateSessionKey(int userId)
        {
            StringBuilder keyChars = new StringBuilder(SessionKeyLength);
            keyChars.Append(userId.ToString());
            while (keyChars.Length < SessionKeyLength)
            {
                int randomCharNum;
                lock (rand)
                {
                    randomCharNum = rand.Next(SessionKeyChars.Length);
                }
                char randomKeyChar = SessionKeyChars[randomCharNum];
                keyChars.Append(randomKeyChar);
            }
            string sessionKey = keyChars.ToString();
            return sessionKey;
        }
    }
}
