using ContactManagerApp.Models;
using ContactManagerApp.Repositories;
using CsvHelper;

namespace ContactManagerApp.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _repository;
        private readonly IParsingService _parser;

        public ManagerService(IManagerRepository repository, IParsingService parser)
        {
            _repository = repository;
            _parser = parser;
        }

        public async Task UploadManagersAsync(Stream fileStream)
        {
            var managers = _parser.ParseManagers(fileStream);

            if (managers.Any())
            {
                await _repository.AddRangeAsync(managers);
                await _repository.SaveChangesAsync();
            }
        }
        public async Task<List<Manager>> GetAllManagersAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Manager?> GetManagerByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task EditManagerAsync(Manager manager)
        {
            _repository.Update(manager);
            await _repository.SaveChangesAsync();
        }
        public async Task DeleteManagerAsync(int id)
        {
            var manager = await _repository.GetByIdAsync(id);
            if (manager != null)
            {
                _repository.Delete(manager);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
