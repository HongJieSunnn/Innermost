using AutoMapper;
using Innemost.LogLife.API.Application.Commands;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
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
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOneRecordCommand> _logger;
        public UpdateOneRecordCommandHandler(ILifeRecordRepository lifeRecordRepository,IMapper mapper, ILogger<UpdateOneRecordCommand> logger)
        {
            _lifeRecordRepository = lifeRecordRepository ?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(UpdateOneRecordCommand request, CancellationToken cancellationToken)
        {
            var recordBeforeUpdate = await _lifeRecordRepository.GetRecordByIdAsync(request.Id);
            var recordToUpdate = _mapper.Map<UpdateOneRecordCommand,LifeRecord>(request);
            if (!recordBeforeUpdate.IsValidatedToUpdate(recordToUpdate))//this method is not allowed to update id,path,publishTime
            {
                return false;
            }

            _logger.LogInformation($"Record with Id {request.Id} starts to update");

            _lifeRecordRepository.Update(recordToUpdate);
            
            return true;
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
