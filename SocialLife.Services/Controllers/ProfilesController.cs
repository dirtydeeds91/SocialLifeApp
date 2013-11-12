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
using System.Web.Http;

namespace SocialLife.Services.Controllers
{
    public class ProfilesController : ApiController
    {
        private IDbContextFactory<DbContext> contextFactory;

        public ProfilesController()
        {
            this.contextFactory = new SocialLifeDbContextFactory();
        }

        public ProfilesController(IDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpPut]
        [ActionName("update")]
        public HttpResponseMessage PutUpdateProfile([FromBody]ProfileModel updatedProfile, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                userEntity.Profile.About = updatedProfile.About;
                userEntity.Profile.Avatar = updatedProfile.Avatar;
                userEntity.Profile.BirthDate = updatedProfile.BirthDate;
                userEntity.Profile.City = updatedProfile.City;
                userEntity.Profile.Country = updatedProfile.Country;
                userEntity.Profile.Gender = updatedProfile.Gender;
                userEntity.Profile.Mood = updatedProfile.Mood;
                userEntity.Profile.PhoneNumber = updatedProfile.PhoneNumber;
                userEntity.Profile.Status.StatusName = updatedProfile.Status;
                userEntity.DisplayName = updatedProfile.DisplayName;

                context.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
        }

        [HttpPut]
        [ActionName("add")]
        public HttpResponseMessage PutAddFriend(int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                var friends = userEntity.Profile.FriendsList;

                var friendsList = friends.Split(' ', ',');

                foreach (var friend in friendsList)
                {
                    if (id == int.Parse(friend))
                    {
                        throw new ArgumentException("Already in friends list.");
                    }
                }

                friends = friends + "," + id.ToString();

                userEntity.Profile.FriendsList = friends;

                context.SaveChanges();

                //SEND NOTIFICATION TO OTHER USER!

                var response = Request.CreateResponse(HttpStatusCode.OK);

                return response;
            }
        }

        [HttpPut]
        [ActionName("remove")]
        public HttpResponseMessage PutRemoveFriend(int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                User removedUser = context.Users.Where(us => us.UserId == id).FirstOrDefault();

                if (userEntity == null || removedUser == null)
                {
                    throw new ArgumentException("No such user.");
                }

                var friends = userEntity.Profile.FriendsList;

                var friendsList = friends.Split(' ', ',');

                string newList = "";

                foreach (var friend in friendsList)
                {
                    if (id != int.Parse(friend))
                    {
                        if (newList == "")
                        {
                            newList = newList + friend;
                        }
                        else
                        {
                            newList = newList + "," + friend;
                        }
                    }
                }

                userEntity.Profile.FriendsList = newList;

                var removedUserFriends = removedUser.Profile.FriendsList;

                var removedFriendsList = removedUserFriends.Split(' ', ',');

                string otherNewList = "";

                foreach (var friend in removedFriendsList)
                {
                    if (id != int.Parse(friend))
                    {
                        if (otherNewList == "")
                        {
                            otherNewList = otherNewList + friend;
                        }
                        else
                        {
                            otherNewList = otherNewList + "," + friend;
                        }
                    }
                }

                removedUser.Profile.FriendsList = otherNewList;

                context.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.OK);

                return response;
            }
        }

        [HttpGet]
        [ActionName("user")]
        public HttpResponseMessage GetUserProfile([FromUri]int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                User foundUser = context.Users.Where(us => us.UserId == id).FirstOrDefault();

                if (foundUser != null)
                {
                    ProfileModel userProfile = new ProfileModel()
                    {
                        UserId = foundUser.UserId,
                        About = foundUser.Profile.About,
                        Avatar = foundUser.Profile.Avatar,
                        BirthDate = foundUser.Profile.BirthDate,
                        City = foundUser.Profile.City,
                        Country = foundUser.Profile.Country,
                        FriendsList = foundUser.Profile.FriendsList,
                        Gender = foundUser.Profile.Gender,
                        DisplayName = foundUser.DisplayName,
                        Mood = foundUser.Profile.Mood,
                        Status = foundUser.Profile.Status.StatusName,
                        PhoneNumber = foundUser.Profile.PhoneNumber,
                        LastLatitude = foundUser.Locations.Last().Latitude,
                        LastLongitute = foundUser.Locations.Last().Longitude,
                        LastLocationDate = foundUser.Locations.Last().Date
                    };

                    var response = Request.CreateResponse<ProfileModel>(HttpStatusCode.OK, userProfile);
                    return response;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("No such user found.");
                }
            }
        }

        [HttpGet]
        [ActionName("search")]
        public HttpResponseMessage GetSearchUsers([FromUri]string username, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                ICollection<User> foundUsers = context.Users.Where(us => us.Username.ToLower().Contains(username.ToLower())
                                                                   || us.DisplayName.ToLower().Contains(username.ToLower()))
                                                                   .ToList();

                if (foundUsers != null)
                {
                    ICollection<UserModel> usersList = new List<UserModel>();

                    foreach (var foundUser in foundUsers)
                    {
                        UserModel userProfile = new UserModel()
                        {
                            Username = foundUser.Username,
                            Id = foundUser.UserId,
                            DisplayName = foundUser.DisplayName
                        };

                        usersList.Add(userProfile);
                    }

                    var response = Request.CreateResponse<ICollection<UserModel>>(HttpStatusCode.OK, usersList);
                    return response;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("No such user found.");
                }
            }
        }

        [HttpPost]
        [ActionName("location")]
        public HttpResponseMessage PostCurrentLocation([FromUri]string longitude, [FromUri]string latitude, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User userEntity = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                if (userEntity == null)
                {
                    throw new ArgumentException("No such logged user.");
                }

                Location userLocation = new Location()
                {
                    UserId = userEntity.UserId,
                    Latitude = latitude,
                    Longitude = longitude,
                    Date = DateTime.Now,
                };

                context.Locations.Add(userLocation);
                context.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
        }
    }
}
