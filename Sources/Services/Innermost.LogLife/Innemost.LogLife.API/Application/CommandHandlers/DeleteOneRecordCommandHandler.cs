using Innemost.LogLife.API.Application.Commands;
using Innermost.LogLife.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.CommandHandlers
{
    public class DeleteOneRecordCommandHandler : IRequestHandler<DeleteOneRecordCommand, bool>
    {
        public Task<bool> Handle(DeleteOneRecordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class IdempotencyDeleteOneRecordCommandHandler:IdempotencyCommandHandler<DeleteOneRecordCommand,bool>
    {
        public IdempotencyDeleteOneRecordCommandHandler(IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdempotencyCommandHandler<DeleteOneRecordCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {

        }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}
