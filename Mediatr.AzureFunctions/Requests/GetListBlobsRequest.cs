using IsolatedMediatr.Models;
using MediatR;

namespace IsolatedMediatr.Queries
{
    public record GetBlobsRequest() : IRequest<IEnumerable<BlobObject>>;
}