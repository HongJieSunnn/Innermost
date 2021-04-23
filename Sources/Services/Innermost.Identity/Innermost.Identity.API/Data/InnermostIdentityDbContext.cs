using Innermost.Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Innermost.Identity.API.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class InnermostIdentityDbContext : IdentityDbContext<InnermostUser>
    {
        public InnermostIdentityDbContext(DbContextOptions<InnermostIdentityDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<InnermostUser>().Property(user => user.Age).HasDefaultValue(1);//Age默认为1岁
            builder.Entity<InnermostUser>().Property(user => user.SelfDescription).HasDefaultValue("Be Yourself");//默认个人描述
            builder.Entity<InnermostUser>().Property(user => user.Birthday).HasDefaultValue("2000-01-01");//默认生日
            builder.Entity<InnermostUser>().Property(user => user.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");//CreateTime默认为CURRENT_TIMESTAMP
            builder.Entity<InnermostUser>().Property(user => user.UpdateTime).HasDefaultValueSql("null");
            builder.Entity<InnermostUser>().Property(user => user.DeleteTime).HasDefaultValueSql("null");
        }
    }
}
