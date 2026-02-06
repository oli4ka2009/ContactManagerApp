using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using ContactManagerApp.Models;
using ContactManagerApp.Models.Dto;
using CsvHelper.TypeConversion;

namespace ContactManagerApp.Services
{
    public class CsvParsingService : IParsingService
    {
        public IEnumerable<Manager> ParseManagers(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim
            };

            using var csv = new CsvReader(reader, config);

            try
            {
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
            catch (HeaderValidationException ex)
            {
                var missingHeaders = string.Join(", ", ex.InvalidHeaders.SelectMany(h => h.Names));
                throw new Exception($"Invalid CSV structure. Missing columns: {missingHeaders}");
            }
            catch (TypeConverterException ex)
            {
                throw new Exception($"Data format error at row {ex.Context.Parser.Row}: {ex.Text} cannot be converted.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing CSV: {ex.Message}");
            }
        }
    }
}
