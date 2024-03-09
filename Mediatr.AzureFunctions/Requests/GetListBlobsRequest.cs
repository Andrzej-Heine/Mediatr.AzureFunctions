using IsolatedMediatr.Models;
using MediatR;

namespace IsolatedMediatr.Requests
{
    public record GetBlobsRequest() : IRequest<IEnumerable<BlobObject>>;
}