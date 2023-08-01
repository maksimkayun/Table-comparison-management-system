#nullable enable

namespace UIFormRDMO.Data.Models
{
    /// <summary>
    /// Работники из штатки
    /// </summary>
    public class PersonDB : IPerson
    {
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public string? DateAttest { get; set; }
        public string? DateMed { get; set; }
    }
}