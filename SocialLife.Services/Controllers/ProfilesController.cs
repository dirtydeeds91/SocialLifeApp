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
                        PhoneNumber = foundUser.Profile.PhoneNumber
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
    }
}
