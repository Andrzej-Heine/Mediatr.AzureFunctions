using IsolatedMediatr.Requests;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IsolatedMediatr.Functions;

public class ProcessQueueMessageFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessQueueMessageFunction> _logger;

    public ProcessQueueMessageFunction(IMediator mediator, ILogger<ProcessQueueMessageFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("ProcessQueueMessageFunction")]
    [BlobOutput("%BlobContainerName%/{datetime:yyyy}/{datetime:MM}/{datetime:dd}/{datetime:HH}/{datetime:mm}/{rand-guid}.json", Connection = "AzureWebJobsStorage")]
    public string Run([QueueTrigger("%QueueName%", Connection = "AzureWebJobsStorage")] string queueMessage)
    {
        try
        {
            _logger.LogInformation($"C# Queue trigger function processed: {queueMessage}");
            return _mediator.Send(new QueueMessageRequest { QueueMessage = queueMessage }).Result;
        }
        catch (System.Exception ex)
        {
            _logger.LogInformation($"[GetListBlobFunction]: {ex}");
            return ex.Message;
        }
    }
}