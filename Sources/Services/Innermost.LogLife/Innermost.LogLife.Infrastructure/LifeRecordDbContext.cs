using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Infrastructure.EntityConfigurations;
using Innermost.LogLife.Infrastructure.Extensions;
using Innermost.LogLife.Infrastructure.Idempotency;
using Innermost.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Innermost.LogLife.Infrastructure
{
    public class LifeRecordDbContext : DbContext, IUnitOfWork
    {
        public DbSet<LifeRecord> LifeRecords { get; set; }
        public DbSet<TextType> TextTypes { get; set; }
        public DbSet<EmotionTag> EmotionTags { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }

        private readonly IMediator _mediator;

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction CurrentTransaction => _currentTransaction;

        public bool HasActiveTrasaction => _currentTransaction != null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LifeRecordEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TextTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EmotionTagEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
        }
        /// <summary>
        /// Used for factory
        /// </summary>
        /// <param name="options"></param>
        public LifeRecordDbContext(DbContextOptions<LifeRecordDbContext> options):base(options)
        {

        }

        public LifeRecordDbContext(DbContextOptions<LifeRecordDbContext> options,IMediator mediatR):base(options)
        {
            _mediator = mediatR ?? throw new ArgumentNullException(nameof(mediatR));
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DisPatchDomainEvents(this);

            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return null;
            _currentTransaction = await Database.BeginTransactionAsync();

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction Id:{transaction.TransactionId} is not current");

            try
            {
                await this.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            //try finnaly 一般用来抛出无法处理的异常，并且需要在抛出异常前执行某些操作，例如这里的资源释放
            try
            {
                //提供了异步的的RollBack方法，但我认为是可以看情况的。一般来说回滚的时候还是同步好点，否则异步的话虽然新线程会等待它结束再执行后面的代码，但实际上也还好，如果直接await了的话
                _currentTransaction.Rollback();
            }
            finally
            {
                if(_currentTransaction!=null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }

    public class LifeRecordDbContextFactory : IDesignTimeDbContextFactory<LifeRecordDbContext>
    {
        public LifeRecordDbContext CreateDbContext(string[] args)
        {
            string connectionString = "";//TODO
            var options = new DbContextOptionsBuilder<LifeRecordDbContext>()
                .UseMySql(connectionString,new MySqlServerVersion(new Version(5,7)));
            return new LifeRecordDbContext(options.Options, new NoMediator());
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(default(object));
            }
        }
    }
}
