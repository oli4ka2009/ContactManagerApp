using ContactManagerApp.Models;

namespace ContactManagerApp.Services
{
    public interface IParsingService
    {
        IEnumerable<Manager> ParseManagers(Stream fileStream);
    }
}
