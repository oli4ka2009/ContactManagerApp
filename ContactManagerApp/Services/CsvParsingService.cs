using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ContactManagerApp.Models;
using ContactManagerApp.Models.Dto;

namespace ContactManagerApp.Services
{
    public class CsvParsingService : IParsingService
    {
        public IEnumerable<Manager> ParseManagers(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                HeaderValidated = null,
                MissingFieldFound = null
            });

            var records = csv.GetRecords<ManagerDto>();

            var managers = new List<Manager>();
            foreach (var record in records)
            {
                managers.Add(new Manager
                {
                    Name = record.Name,
                    DateOfBirth = record.DateOfBirth,
                    IsMarried = record.IsMarried,
                    Phone = record.Phone,
                    Salary = record.Salary
                });
            }

            return managers;
        }
    }
}
