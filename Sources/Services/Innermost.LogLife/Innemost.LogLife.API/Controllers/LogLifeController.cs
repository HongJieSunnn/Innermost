using Innemost.LogLife.API.Application.Commands;
using Innemost.LogLife.API.Models;
using Innemost.LogLife.API.Queries;
using Innemost.LogLife.API.Queries.Model;
using Innemost.LogLife.API.Services.IdentityServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogLifeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly ILifeRecordQueries _lifeRecordQueries;
        private readonly ILogger<LogLifeController> _logger;
        public LogLifeController(IMediator mediator,IIdentityService identityService,ILifeRecordQueries lifeRecordQueries,ILogger<LogLifeController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService=identityService?? throw new ArgumentNullException(nameof(identityService));
            _lifeRecordQueries=lifeRecordQueries?? throw new ArgumentNullException(nameof(lifeRecordQueries));
            _logger = logger?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Api to create one record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="requestId">from getway</param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOneRecordAsync([FromBody] CreateOneRecordCommand record,[FromHeader(Name ="x-requestid")] string requestId)
        {
            bool commandResult = false;

            if(Guid.TryParse(requestId,out Guid guid)&&guid!=Guid.Empty)
            {
                var createCommand = new IdempotencyCommand<CreateOneRecordCommand, bool>(record, guid);

                _logger.LogInformation("");//TODO

                commandResult =await _mediator.Send(createCommand);
            }

            if (!commandResult)
                return BadRequest();//TODO 或者直接抛异常 然后 filter 

            return Ok();
        }

        /// <summary>
        /// Find record detail by recordId
        /// </summary>
        /// <param name="recordId">The id of record which is wanted to be find.Please ensure the recordId is available.</param>
        /// <returns>The corresponding LifeRecord which packages by record in C#9 and is different from LifeRecord entity in domain layer</returns>
        [Route("record/{recordId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(LifeRecord), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<LifeRecord>> GetRecordByIdAsync(int recordId)//TODO Add Attribute to ensure the recordId belongs to user who requests
        {
            var record = await _lifeRecordQueries.FindRecordByRecordId(recordId);

            if (record == null)
                return BadRequest("");

            return Ok(record);
        }

        /// <summary>
        /// Api to get records summary in path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Route("summary/{path:string}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecordSummary>),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<RecordSummary>>> GetRecordsSummaryByPathAsync(string path)
        {
            var userId = _identityService.GetUserId();

            var records= await _lifeRecordQueries.FindRecordsByPathAsync(userId, path);

            if (records == null)
            {
                var pathIsExisted = await _lifeRecordQueries.IsPathExistedAsync(userId,path);
                return pathIsExisted ? Ok(new List<RecordSummary>()) : NotFound($"{path} of User {userId} is not existed.");
            }

            return Ok(records);
        }

        [Route("summary/time")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RecordSummary>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<RecordSummary>>> GetRecordsSummaryByPublishTimeAsync([FromBody]DateTimeToFind dateTimeToFind)
        {
            var userId = _identityService.GetUserId();

            var records = await _lifeRecordQueries.FindRecordsByPublishTimeAsync(userId, dateTimeToFind);

            return records==null?Ok(records):Ok(new List<RecordSummary>());
        }

        [Route("update")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateOneRecordAsync([FromBody] UpdateOneRecordCommand updateOneRecordCommand, [FromHeader(Name = "x-requestid")] string requestId)//TODO 也许 Update 可以细化?
        {
            var commandResult = false;

            if(Guid.TryParse(requestId,out Guid guid)&&guid!=Guid.Empty)
            {
                var updateCommand = new IdempotencyCommand<UpdateOneRecordCommand, bool>(updateOneRecordCommand,guid);

                _logger.LogInformation("");//TODO

                commandResult =await _mediator.Send(updateCommand);
            }

            if (!commandResult)
                return BadRequest("Record can not update ID,PublishTime,Path by api \"update\" ");

            return Ok();
        }

        [HttpDelete]
        [Route("delete/{recordId:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteOneRecordAsync(int recordId, [FromHeader(Name = "x-requestid")] string requestId)//TODO Add Attribute to ensure the recordId belongs to user who requests
        {
            var commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var updateCommand = new IdempotencyCommand<DeleteOneRecordCommand, bool>(new DeleteOneRecordCommand(recordId), guid);

                _logger.LogInformation("");//TODO

                commandResult = await _mediator.Send(updateCommand);
            }

            if (!commandResult)
                return BadRequest($"Record with Id {recordId} is not existed.");

            return Ok();
        }

        [HttpDelete]
        [Route("delete/{path:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteRecordsByPathAsync(string path, [FromHeader(Name = "x-requestid")] string requestId)//TODO Add Attribute to ensure the path belongs to user who requests
        {
            var commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var updateCommand = new IdempotencyCommand<DeleteRecordsByPathCommand, bool>(new DeleteRecordsByPathCommand(path), guid);

                _logger.LogInformation("");//TODO

                commandResult = await _mediator.Send(updateCommand);
            }

            if (!commandResult)
                return BadRequest();

            return Ok();
        }
    }
}
