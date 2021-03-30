using Innermost.EventBusInnermost.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord.Services
{
    public class IntegrationEventRecordService : IIntegrationEventRecordService,IDisposable
    {
        private readonly DbConnection _dbConnection;
        private readonly IntegrationEventRecordDbContext _dbContext;
        private readonly List<Type> _eventTypes;
        private volatile bool disposed;
        public IntegrationEventRecordService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

            _dbContext = new IntegrationEventRecordDbContext(
                new DbContextOptionsBuilder<IntegrationEventRecordDbContext>()
                        .UseMySql(_dbConnection,new MySqlServerVersion(new Version(5,7)))
                        .Options);

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith("IntegrationEvent"))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventRecord>> RetrieveEventsByEventContentsToPublishAsync(Guid transactionId)
        {
            var transactionIdStr = transactionId.ToString();

            var recordsOfTransaction =await _dbContext.IntegrationEventRecords.Where(i => i.TransactionId == transactionIdStr&&i.State==EventState.NotPublished).ToListAsync();

            if(recordsOfTransaction!=null)
            {
                return recordsOfTransaction.OrderBy(i => i.CreateTime).Select(i => i.DeserializeIntegrationEventFromEventContent(_eventTypes.First(t => t.Name == i.EventTypeShortName)));
            }
            return new List<IntegrationEventRecord>();
        }

        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if(transaction==null)  throw new ArgumentNullException(nameof(transaction));

            var eventRecord = new IntegrationEventRecord(@event, transaction.TransactionId);

            _dbContext.Database.UseTransaction(transaction.GetDbTransaction());
            _dbContext.IntegrationEventRecords.Add(eventRecord);

            return _dbContext.SaveChangesAsync();
        }

        public Task MarkEventAsInProcessAsync(Guid eventId)
        {
            return UpdateEventState(eventId, EventState.InProcess);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventState(eventId, EventState.Published);
        }

        public Task MarkEventAsPublishedFailedAsync(Guid eventId)
        {
            return UpdateEventState(eventId, EventState.PublishedFailed);
        }

        private Task UpdateEventState(Guid eventId,EventState state)
        {
            var eventRecord = _dbContext.IntegrationEventRecords.Single(i => i.EventId == eventId);

            if (eventRecord.State == EventState.InProcess)
                eventRecord.TimesSend++;

            eventRecord.State = state;
            _dbContext.IntegrationEventRecords.Update(eventRecord);

            return _dbContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool canDispose)
        {
            if(!disposed)
            {
                if(canDispose)
                {
                    _dbContext?.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(canDispose: true);
            GC.SuppressFinalize(this);
        }
    }
}
