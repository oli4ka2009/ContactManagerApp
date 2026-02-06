using CsvHelper.Configuration.Attributes;

namespace ContactManagerApp.Models.Dto
{
    public class ManagerDto
    {
        [Name("Name")]
        public string Name { get; set; }

        [Name("Date of birth")]
        public DateOnly DateOfBirth { get; set; }

        [Name("Married")]
        public bool IsMarried { get; set; }

        [Name("Phone")]
        public string Phone { get; set; }

        [Name("Salary")]
        public decimal Salary { get; set; }
    }
}
