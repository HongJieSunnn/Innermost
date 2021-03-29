using Innemost.LogLife.API.Application.IntegrationEvents;
using Innermost.LogLife.Infrastructure;
using Innermost.TypeExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly LifeRecordDbContext _dbContext;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly ILogLifeIntegrationEventService _integrationEventService;
        public TransactionBehavior(LifeRecordDbContext context,ILogger<TransactionBehavior<TRequest, TResponse>> logger,ILogLifeIntegrationEventService integrationEventService)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(LifeRecordDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(ILogger));
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(ILogLifeIntegrationEventService));
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var requestName = request.GetGenericTypeName();

            try
            {
                if(_dbContext.HasActiveTrasaction)
                {
                    return await next();
                }

                var stragegy = _dbContext.Database.CreateExecutionStrategy();

                await stragegy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction =await _dbContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext",transaction.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, requestName, request);

                        response = await next.Invoke();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, requestName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    await _integrationEventService.PublishEventsAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Handling transaction for {requestName} ({request})");
                throw;
            }
        }
    }
}
