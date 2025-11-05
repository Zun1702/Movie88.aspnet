using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IComboRepository
{
    Task<List<ComboModel>> GetAllAsync();
    Task<List<ComboModel>> GetCombosByIdsAsync(List<int> comboIds, CancellationToken cancellationToken = default);
}
