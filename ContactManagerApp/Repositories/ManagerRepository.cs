using ContactManagerApp.Data;
using ContactManagerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApp.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ApplicationDbContext _context;

        public ManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Manager>> GetAllAsync()
        {
            return await _context.Managers.AsNoTracking().ToListAsync();
        }
        public async Task<Manager?> GetByIdAsync(int id)
        {
            return await _context.Managers.FindAsync(id);
        }
        public async Task AddRangeAsync(IEnumerable<Manager> managers)
        {
            await _context.Managers.AddRangeAsync(managers);
        }
        public void Update(Manager manager)
        {
            _context.Managers.Update(manager);
        }
        public void Delete(Manager manager)
        {
            _context.Managers.Remove(manager);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
