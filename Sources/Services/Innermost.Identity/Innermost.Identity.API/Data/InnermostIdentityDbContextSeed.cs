using Innermost.Identity.API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.Identity.API.Data
{
    public class InnermostIdentityDbContextSeed
    {
        public async Task SeedAsync(InnermostIdentityDbContext context,IConfiguration configuration)
        {
            await context.Users.AddRangeAsync(DefaultUsers());
            await context.SaveChangesAsync();
        }

        public List<InnermostUser> DefaultUsers()
        {
            return new List<InnermostUser>()
            {
                new InnermostUser()
                {
                    UserName="HongJieSun",
                    NormalizedUserName="HONGJIESUN",
                    Email="457406475@qq.com",
                    NormalizedEmail="457406475@QQ.COM",
                    Age=20,
                    Gender="MALE",
                    NickName="Deficienthonnn",
                    School="Nanjing Tech University",
                    Province="福建省",
                    City="福州市",
                    SelfDescription="I am HongJieSun",
                    Birthday="2000-08-26",
                    CreateTime=DateTime.Now,
                    PhoneNumber="18506013757",
                }
            };
        }
    }
}
