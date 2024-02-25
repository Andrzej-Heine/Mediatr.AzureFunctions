using MediatR;

namespace IsolatedMediatr.Requests
{
    public class QueueMessageRequest : IRequest<string>
    {
        public string QueueMessage { get; set; }
    }
}
