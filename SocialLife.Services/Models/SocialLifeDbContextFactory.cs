using SocialLife.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class SocialLifeDbContextFactory : IDbContextFactory<DbContext>
    {
        public DbContext Create()
        {
            return new SocialLifeContext();
        }
    }
}