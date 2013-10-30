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

            var user = new User();
            user.Username = "Pesho";
            user.Password = "1234567890123456789012345678901234567890";
            db.Users.Add(user);
            db.SaveChanges();
        }
    }
}
