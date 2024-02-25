using IsolatedMediatr.Queries;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IsolatedMediatr.Functions
{
    public class GetListBlobFunction
    {
        private readonly ILogger<GetListBlobFunction> _logger;
        private readonly IMediator _mediator;

        public GetListBlobFunction(IMediator mediator, ILogger<GetListBlobFunction> logger, IConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Function("GetListBlobFunction")]
        public async Task<HttpResponseData> GetListBlobs([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "ListBlobs")]
            HttpRequestData request)
        {
            try
            {
                _logger.LogInformation("GetListBlobFunction");
                var blobs = await _mediator.Send(new GetBlobsRequest());
                var response = request.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(blobs);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogInformation($"[GetListBlobFunction]: {ex}");
                var response = request.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}
