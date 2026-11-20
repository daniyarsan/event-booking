using System.Data;

using CSharpFunctionalExtensions;
using EventBooking.Domain.Errors;

namespace EventBooking.Application.Ports;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken, IsolationLevel? level = null);
    Task<UnitResult<Error>> SaveChagnesAsync(CancellationToken cancellationToken);
}
