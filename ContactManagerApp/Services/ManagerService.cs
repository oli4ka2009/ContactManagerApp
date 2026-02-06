using ContactManagerApp.Models;
using ContactManagerApp.Repositories;
using CsvHelper;
using System.ComponentModel.DataAnnotations;

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
            var managers = _parser.ParseManagers(fileStream).ToList();

            ValidateManagersList(managers);

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
        private void ValidateManagersList(List<Manager> managers)
        {
            var errors = new List<string>();
            int rowIndex = 2;

            foreach (var manager in managers)
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(manager);

                if (!Validator.TryValidateObject(manager, context, validationResults, validateAllProperties: true))
                {
                    var messages = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                    errors.Add($"Row {rowIndex} ({manager.Name}): {messages}");
                }
                rowIndex++;
            }

            if (errors.Any())
            {
                throw new Exception($"Validation failed:\n{string.Join("\n", errors)}");
            }
        }
    }
}
