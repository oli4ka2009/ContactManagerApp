using ContactManagerApp.Models;
using ContactManagerApp.Repositories;

namespace ContactManagerApp.Services
{
    public interface IManagerService
    {
        Task UploadManagersAsync(Stream fileStream);
        Task<List<Manager>> GetAllManagersAsync();
        Task<Manager?> GetManagerByIdAsync(int id);
        Task EditManagerAsync(Manager manager);
        Task DeleteManagerAsync(int id);
    }
}
