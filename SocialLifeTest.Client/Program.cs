using SocialLife.Data;
using SocialLife.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SocialLifeTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SocialLifeContext();

            var status = new Status();
            status.StatusName = "Online";
            status.StatusId = 1;
            db.Statuses.Add(status);
            db.SaveChanges();
        }
    }
}
