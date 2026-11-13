using CSharpFunctionalExtensions;

using EventBooking.Domain.Errors;

namespace EventBooking.Application.Ports;

public interface ITransactionScope : IAsyncDisposable
{
    Task<UnitResult<Error>> CommitAsync(CancellationToken cancellationToken);
    Task<UnitResult<Error>> RollbackAsync(CancellationToken cancellationToken);
}
