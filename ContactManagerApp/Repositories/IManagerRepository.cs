using ContactManagerApp.Models;

namespace ContactManagerApp.Repositories
{
    public interface IManagerRepository
    {
        Task<List<Manager>> GetAllAsync();
        Task<Manager?> GetByIdAsync(int id);
        Task AddRangeAsync(IEnumerable<Manager> managers);
        void Update(Manager manager);
        void Delete(Manager manager);
        Task SaveChangesAsync();
    }
}
