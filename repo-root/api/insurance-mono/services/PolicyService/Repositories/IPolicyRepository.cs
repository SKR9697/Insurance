using PolicyService.Models;

namespace PolicyService.Repositories
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<Policy>> GetAllAsync();
        Task<Policy?> GetAsync(int id);
        Task<int> CreateAsync(Policy p);
        Task<bool> UpdateAsync(Policy p);
        Task<bool> DeleteAsync(int id);
    }
}
