using IsolatedMediatr.DateStore;
using IsolatedMediatr.Models;
using IsolatedMediatr.Queries;
using MediatR;

namespace IsolatedMediatr.Handlers
{
    public class GetListBlobsHandler : IRequestHandler<GetBlobsRequest, IEnumerable<BlobObject>>
    {
        private readonly BlobDataStore _blobDataStore;

        public GetListBlobsHandler(BlobDataStore blobDataStore) => _blobDataStore = blobDataStore;

        public async Task<IEnumerable<BlobObject>> Handle(GetBlobsRequest request,
            CancellationToken cancellationToken) => await _blobDataStore.GetAllBlobs();
    }
}
