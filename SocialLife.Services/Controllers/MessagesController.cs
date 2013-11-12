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
    public class MessagesController : ApiController
    {
        private IDbContextFactory<DbContext> contextFactory;

        public MessagesController()
        {
            this.contextFactory = new SocialLifeDbContextFactory();
        }

        public MessagesController(IDbContextFactory<DbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        [HttpGet]
        [ActionName("getpm")]
        public HttpResponseMessage GetUserMessages(int id, [FromUri]string sessionKey)//, [FromUri]int userId)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User currentUser = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                User correspondingUser = context.Users.Where(us => us.UserId == id).FirstOrDefault();

                if (currentUser == null || correspondingUser == null)
                {
                    throw new ArgumentOutOfRangeException("No such user or invalid key.");
                }

                var sentMessages = context.Messages.Where(msg => msg.SenderId == currentUser.UserId && msg.ReceiverId == correspondingUser.UserId);
                var receivedMessages = context.Messages.Where(msg => msg.SenderId == correspondingUser.UserId && msg.ReceiverId == currentUser.UserId);
                var messages = new List<Message>(); //context.Messages.Where(msg => (msg.SenderId == currentUser.UserId && msg.ReceiverId == id)
                    //|| (msg.SenderId == id && msg.ReceiverId == currentUser.UserId));//
                foreach (var message in sentMessages)
                {
                    messages.Add(message);
                }
                foreach (var message in receivedMessages)
                {
                    messages.Add(message);
                }
                if (messages == null)
                {
                    throw new ArgumentNullException("No messages found.");
                }
                //messages.AsQueryable().OrderBy(msg => msg.MessageDate);

                var orderedMessages = messages.AsQueryable().OrderBy(msg => msg.MessageDate);

                ICollection<MessageModel> messagesModel = new List<MessageModel>();

                foreach (var singleMessage in orderedMessages)
                {
                    //if the receiver gets a message, it changes the status to "read"
                    //TODO: FIX THE STATUS ID!!!!
                    var readMessageStatus = context.Statuses.Where(st => st.StatusName == "Read").First().StatusId;

                    if (singleMessage.StatusId != readMessageStatus && singleMessage.SenderId == id)
                    {
                        singleMessage.StatusId = readMessageStatus;
                        context.SaveChanges();
                    }

                    messagesModel.Add(new MessageModel()
                    {
                        Content = singleMessage.MessageContent,
                        Date = singleMessage.MessageDate,
                        Sender = singleMessage.Sender.DisplayName,
                        Receiver = singleMessage.Receiver.DisplayName,
                        Status = singleMessage.Status.StatusName
                    });
                }

                HttpResponseMessage successfulResponse = Request.CreateResponse<ICollection<MessageModel>>(HttpStatusCode.Created, messagesModel);

                //this.SendNotification(id);
                return successfulResponse;
            }
        }

        [HttpPost]
        [ActionName("postpm")]
        public HttpResponseMessage PostUserMessage([FromUri]string sessionKey, [FromBody]MessageModel message)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                    var receiver = context.Users.Where(us => us.UserId == message.ReceiverId).FirstOrDefault();

                    if (sender == null || receiver == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user, event or invalid key.");
                    }

                    //CHECK IF RECEIVER IS FRIEND
                    if (!sender.Profile.FriendsList.Contains(receiver.UserId.ToString()))
                    {
                        throw new ArgumentOutOfRangeException("This user is not in your friends list.");
                    }

                    Message messageToPush = new Message()
                    {
                        MessageContent = message.Content,
                        MessageDate = message.Date,
                        Sender = sender,
                        Receiver = receiver,
                        StatusId = 1,
                    };

                    context.Messages.Add(messageToPush);
                    context.SaveChanges();

                    MessageModel resultMessage = new MessageModel()
                    {
                        Content = messageToPush.MessageContent,
                        Date = messageToPush.MessageDate,
                        Sender = messageToPush.Sender.DisplayName,
                        Receiver = messageToPush.Receiver.DisplayName
                    };

                    HttpResponseMessage successfulResponse = Request.CreateResponse<MessageModel>(HttpStatusCode.Created, resultMessage);

                    //this.SendNotification(id);
                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpGet]
        [ActionName("getevent")]
        public HttpResponseMessage GetEventMessages(int id, [FromUri]string sessionKey)//, [FromUri]int userId)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                User currentUser = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                Event chosenEvent = context.Events.Where(ev => ev.EventId == id).FirstOrDefault();

                if (currentUser == null || chosenEvent == null)
                {
                    throw new ArgumentOutOfRangeException("No such user, event or invalid key.");
                }

                var messages = context.Messages.Where(msg => msg.EventId == id);
                if (messages == null)
                {
                    throw new ArgumentNullException("No messages found.");
                }

                var orderedMessages = messages.AsQueryable().OrderBy(msg => msg.MessageDate);

                ICollection<MessageModel> messagesModel = new List<MessageModel>();

                foreach (var singleMessage in orderedMessages)
                {
                    //if the receiver gets a message, it changes the status to "read"
                    var readMessageStatus = context.Statuses.Where(st => st.StatusName == "Read").First().StatusId;

                    if (singleMessage.StatusId != readMessageStatus && singleMessage.SenderId == id)
                    {
                        singleMessage.StatusId = readMessageStatus;
                        context.SaveChanges();
                    }

                    messagesModel.Add(new MessageModel()
                    {
                        Content = singleMessage.MessageContent,
                        Date = singleMessage.MessageDate,
                        Sender = singleMessage.Sender.DisplayName,
                        Event = chosenEvent.EventName,
                        Status = singleMessage.Status.StatusName
                    });
                }

                HttpResponseMessage successfulResponse = Request.CreateResponse<ICollection<MessageModel>>(HttpStatusCode.Created, messagesModel);

                //this.SendNotification(id);
                return successfulResponse;
            }
        }

        [HttpPost]
        [ActionName("postevent")]
        public HttpResponseMessage PostEventMessage([FromUri]string sessionKey, [FromBody]MessageModel message)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();
                    var messageEvent = context.Events.Where(ev => ev.EventId == message.EventId).FirstOrDefault();

                    if (sender == null || messageEvent == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user, event or invalid key.");
                    }
                    Message messageToPush = new Message()
                    {
                        MessageContent = message.Content,
                        MessageDate = message.Date,
                        Sender = sender,
                        EventId = messageEvent.EventId,
                        StatusId = 1,
                    };

                    context.Messages.Add(messageToPush);
                    context.SaveChanges();

                    MessageModel resultMessage = new MessageModel()
                    {
                        Content = messageToPush.MessageContent,
                        Date = messageToPush.MessageDate,
                        Sender = messageToPush.Sender.DisplayName,
                        Event = messageEvent.EventName
                    };

                    HttpResponseMessage successfulResponse = Request.CreateResponse<MessageModel>(HttpStatusCode.Created, resultMessage);

                    //this.SendNotification(id);
                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }

        [HttpPost]
        [ActionName("free")]
        public HttpResponseMessage PostFreeFriends([FromUri]string sessionKey, [FromBody]MessageModel message)
        {
            var context = new SocialLifeContext();
            using (context)
            {
                if (ModelState.IsValid)
                {
                    User sender = context.Users.Where(us => us.SessionKey == sessionKey).FirstOrDefault();

                    var friends = sender.Profile.FriendsList.Split(' ', ',');

                    if (sender == null)
                    {
                        throw new ArgumentOutOfRangeException("No such user, event or invalid key.");
                    }
                    foreach (var friend in friends)
                    {
                        int friendId = int.Parse(friend);

                        var receiver = context.Users.Where(us => us.UserId == friendId).FirstOrDefault();

                        if (receiver.Profile.Status.StatusName == "Free")
                        {
                            Message messageToPush = new Message()
                            {
                                MessageContent = message.Content,
                                MessageDate = message.Date,
                                Sender = sender,
                                Receiver = receiver,
                                StatusId = 1,
                            };

                            context.Messages.Add(messageToPush);
                        }
                       
                    }
                    context.SaveChanges();

                    HttpResponseMessage successfulResponse = Request.CreateResponse(HttpStatusCode.Created);

                    return successfulResponse;
                }

                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }
        }
    }
}
