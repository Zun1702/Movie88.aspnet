namespace Movie88.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task Commit(CancellationToken cancellationToken = default);
    }
}
