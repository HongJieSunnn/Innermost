using Innermost.LogLife.Infrastructure.Idempotency;
using Innermost.TypeExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    public class IdempotencyCommandHandler<TCommand, TResult> : IRequestHandler<IdempotencyCommand<TCommand, TResult>, TResult>
        where TCommand:IRequest<TResult>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdempotencyCommandHandler<TCommand, TResult>> _logger;
        public IdempotencyCommandHandler(IMediator mediator,IRequestManager requestManager,ILogger<IdempotencyCommandHandler<TCommand, TResult>> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
            _logger=logger?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<TResult> Handle(IdempotencyCommand<TCommand, TResult> request, CancellationToken cancellationToken)
        {
            var exsistTag =await _requestManager.ExistAsync(request.Id);
            if(exsistTag)
            {
                return CreateDefaultResult();
            }
            else
            {
                await _requestManager.CreateRequestForCommandAsync<TCommand>(request.Id);
                try
                {
                    var command = request.Command;
                    var commandType = command.GetGenericTypeName();

                    var result= await _mediator.Send(command, cancellationToken);
                    //TODO Log
                    return result;
                }
                catch (Exception)
                {
                    return default(TResult);
                }
            }
        }

        protected virtual TResult CreateDefaultResult()
        {
            return default(TResult);
        }
    }
}
