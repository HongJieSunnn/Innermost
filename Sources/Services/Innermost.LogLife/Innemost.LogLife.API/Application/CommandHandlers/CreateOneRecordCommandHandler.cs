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
    public class CreateOneRecordCommandHandler : IRequestHandler<CreateOneRecordCommand,bool>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOneRecordCommandHandler> _logger;
        public CreateOneRecordCommandHandler(ILifeRecordRepository lifeRecordRepository,IMapper mapper,ILogger<CreateOneRecordCommandHandler> logger)
        {
            _lifeRecordRepository = lifeRecordRepository ?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _logger=logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(CreateOneRecordCommand request, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<LifeRecord>(request);

            _logger.LogInformation("Create Record {@LifeRecord}", record);

            await _lifeRecordRepository.AddAsync(record);

            return await _lifeRecordRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }

    /// <summary>
    /// We need to create corresponding IdempotencyHandler for each commandHandler so thah medietR can detect the IdempotencyHandler even commandHandler will handle command detailly 
    /// and concrete IdempotencyHandler should not override Handle method because all the IdempotencyHandlers have same logic of Handle method and it's realise in base class IdempotencyCommandHandler.
    /// </summary>
    public class IdempotencyCreateOneRecordCommandHandler:IdempotencyCommandHandler<CreateOneRecordCommand,bool>
    {
        public IdempotencyCreateOneRecordCommandHandler(
            IMediator mediator, 
            IRequestManager requestManager, 
            ILogger<IdempotencyCommandHandler<CreateOneRecordCommand, bool>> logger) 
            :base(mediator,requestManager,logger)
        {
            
        }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}
