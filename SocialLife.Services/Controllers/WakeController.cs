using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialLife.Services.Controllers
{
    public class WakeController : ApiController
    {
        [HttpGet]
        [ActionName("wake")]
        public string GetSearchEvents()
        {
            return "Done";
        }
    }
}
