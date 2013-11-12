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
    public class EventsController : ApiController
    {
        private IDbContextFactory<DbContext> contextFactory;

        public EventsController()
        {
            this.contextFactory = new SocialLifeDbContextFactory();
        }

        public EventsController(IDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpGet]
        [ActionName("get")]
        public HttpResponseMessage GetEventInfo(int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                    if (sender == null || chosenEvent == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user or invalid key.");
                    }

                    var usersList = chosenEvent.Users.ToList();
                    //TODO - Serialization of users!
                    EventModel eventInfo = new EventModel()
                    {
                        Name = chosenEvent.EventName,
                        Content = chosenEvent.EventContent,
                        CreatorName = chosenEvent.Users.First().DisplayName,
                        Date = chosenEvent.EventDate,
                        Longitude = chosenEvent.EventLocation.Longitude,
                        Latitude = chosenEvent.EventLocation.Latitude,
                        Status = chosenEvent.EventStatus.StatusName,
                        UsersList = new List<UserModel>()
                    };

                    if (usersList != null)
                    {
                        foreach (var user in usersList)
                        {
                            var newUser = new UserModel()
                            {
                                Username = user.Username,
                                DisplayName = user.DisplayName,
                                Id = user.UserId
                            };
                            eventInfo.UsersList.Add(newUser);
                        }
                    }

                    HttpResponseMessage successfulResponse = Request.CreateResponse<EventModel>(HttpStatusCode.OK, eventInfo);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage PostCreateEvent([FromUri]string sessionKey, [FromBody]EventModel newEvent)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    if (sender == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user or invalid key.");
                    }

                    Event eventToAdd = new Event()
                    {
                        EventName = newEvent.Name,
                        EventContent = newEvent.Content,
                        EventDate = newEvent.Date,
                        Messages = new List<Message>(),
                        Users = new List<User>(),
                        //WAIT WUUUUUUT?
                        EventStatus = context.Statuses.First(),
                        EventLocation = new Location()
                        {
                            Latitude = newEvent.Latitude,
                            Longitude = newEvent.Longitude,
                            UserId = sender.UserId,
                            Date = newEvent.Date
                        }
                    };
                    eventToAdd.Users.Add(sender);
                    context.Events.Add(eventToAdd);
                    context.SaveChanges();

                    EventModel resultEvent = new EventModel()
                    {
                        Name = eventToAdd.EventName,
                        Content = eventToAdd.EventContent,
                        CreatorName = eventToAdd.Users.First().DisplayName,
                        Date = eventToAdd.EventDate,
                        Latitude = eventToAdd.EventLocation.Latitude,
                        Longitude = eventToAdd.EventLocation.Longitude,
                        Status = eventToAdd.EventStatus.StatusName
                    };

                    HttpResponseMessage successfulResponse = Request.CreateResponse<EventModel>(HttpStatusCode.Created, resultEvent);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpPut]
        [ActionName("add")]
        public HttpResponseMessage PutAddUserToEvent(int id, [FromUri]string sessionKey, [FromUri]int? userId)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                    if (sender == null || chosenEvent == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user or invalid key.");
                    }

                    if (userId == null)
                    {
                        chosenEvent.Users.Add(sender);
                        context.SaveChanges();
                    }
                    else
                    {
                        var friend = context.Users.Where(us => us.UserId == userId).FirstOrDefault();
                        //ADD CHECK TO SEE IF USER IS FRIEND
                        chosenEvent.Users.Add(friend);
                        context.SaveChanges();
                    }
                    
                    HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.OK);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpPut]
        [ActionName("remove")]
        public HttpResponseMessage PutRemoveUserFromEvent(int id, [FromUri]string sessionKey, [FromUri]int userId)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    User userToRemove = context.Users.Where(us => us.UserId == userId).FirstOrDefault();

                    Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                    if (sender == null || chosenEvent == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user or invalid key.");
                    }

                    if (chosenEvent.Users.First() != sender && sender != userToRemove)
                    {
                        throw new ArgumentException("You don't have permission to do that.");
                    }

                    if (chosenEvent.Users.First() == sender ^ sender == userToRemove)
                    {
                        chosenEvent.Users.Remove(userToRemove);
                    }

                    HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.OK);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpPut]
        [ActionName("update")]
        public HttpResponseMessage PutUpdateEvent(int id, [FromUri]string sessionKey, [FromBody]EventModel updatedEvent)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                    if (sender == null || chosenEvent == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user or invalid key.");
                    }
                    if (sender != chosenEvent.Users.First())
                    {
                        throw new ArgumentException("You do not have permission to do that.");
                    }
                    chosenEvent.EventContent = updatedEvent.Content;
                    chosenEvent.EventDate = updatedEvent.Date;
                    chosenEvent.EventName = updatedEvent.Name;
                    chosenEvent.StatusId = int.Parse(updatedEvent.Status);
                    if (updatedEvent.Latitude != null && updatedEvent.Longitude != null)
                    {
                        chosenEvent.EventLocation = new Location()
                        {
                            Date = updatedEvent.Date,
                            Latitude = updatedEvent.Latitude,
                            Longitude = updatedEvent.Longitude,
                        };
                    }
                    context.SaveChanges();

                    HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.OK);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteEvent(int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                    if (sender != chosenEvent.Users.First())
                    {
                        throw new ArgumentException("You do not have permission to do that.");
                    }

                    context.Events.Remove(chosenEvent);

                    context.SaveChanges();

                    HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.OK);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpGet]
        [ActionName("search")]
        public HttpResponseMessage GetSearchEvents([FromUri]string name, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    ICollection<Event> foundEvents = context.Events.Where(ev => ev.EventName.ToLower().Contains(name.ToLower()) 
                                                                          && ev.EventStatus.StatusName == "Public")
                                                                          .ToList();

                    if (sender == null || foundEvents.Count == 0)
                    {
                        throw new ArgumentOutOfRangeException("No such user, no found events or invalid key.");
                    }

                    ICollection<EventModel> results = new List<EventModel>();

                    foreach (var chosenEvent in foundEvents)
                    {
                        EventModel eventInfo = new EventModel()
                        {
                            EventId = chosenEvent.EventId,
                            Name = chosenEvent.EventName,
                            CreatorName = chosenEvent.Users.First().DisplayName,
                            Date = chosenEvent.EventDate
                        };

                        results.Add(eventInfo);
                    }
                    
                    HttpResponseMessage successfulResponse = Request.CreateResponse<ICollection<EventModel>>(HttpStatusCode.OK, results);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpGet]
        [ActionName("user")]
        public HttpResponseMessage GetSearchEvents(int id, [FromUri]string sessionKey)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    ICollection<Event> foundEvents = context.Events.Where(ev => ev.Users.FirstOrDefault().UserId == id).ToList();

                    if (sender == null || foundEvents.Count == 0)
                    {
                        throw new ArgumentOutOfRangeException("No such user, no found events or invalid key.");
                    }

                    ICollection<EventModel> results = new List<EventModel>();

                    foreach (var chosenEvent in foundEvents)
                    {
                        EventModel eventInfo = new EventModel()
                        {
                            EventId = chosenEvent.EventId,
                            Name = chosenEvent.EventName,
                            Date = chosenEvent.EventDate
                        };

                        results.Add(eventInfo);
                    }

                    HttpResponseMessage successfulResponse = Request.CreateResponse<ICollection<EventModel>>(HttpStatusCode.OK, results);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }
    }
}
