using Innermost.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure.Extensions
{
    public static class MediatRDispatchExtension
    {
        public static async Task DisPatchDomainEvents(this IMediator mediatR,LifeRecordDbContext context)
        {
            var entities=context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());

            var domainEvents = entities.SelectMany(e => e.Entity.DomainEvents).ToList();

            entities.ToList().ForEach(e => e.Entity.ClearDomainEvents());

            foreach(var domainEvent in domainEvents)
            {
                await mediatR.Publish(domainEvent);
            }
        }
    }
}
