using GdNetDDD.Common;
using Raven.Client.Documents.Session;

namespace GdNetRavenDbExt;

public class RavenDbUnitOfWork(IAsyncDocumentSession documentSession) : IUnitOfWork
{
    public void Dispose()
    {
        // Do not dispose the documentSession, it maybe use in other places within the scope
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}