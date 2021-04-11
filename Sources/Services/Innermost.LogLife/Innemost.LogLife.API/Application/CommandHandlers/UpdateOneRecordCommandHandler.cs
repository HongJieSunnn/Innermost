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
    public class UpdateOneRecordCommandHandler : IRequestHandler<UpdateOneRecordCommand, bool>
    {
        public Task<bool> Handle(UpdateOneRecordCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class IdempotencyUpdateOneRecordCommandHandler:IdempotencyCommandHandler<UpdateOneRecordCommand,bool>
    {
        public IdempotencyUpdateOneRecordCommandHandler(IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdempotencyCommandHandler<UpdateOneRecordCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {

        }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}
