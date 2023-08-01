
namespace UIFormRDMO.Data.Models
{
    /// <summary>
    /// Работники из списков инструкторов
    /// </summary>
    public class PersonList : IPerson
    {
        public PersonList(IPerson p)
        {
            FullName = p.FullName;
            Position = p.Position;
            DateAttest = p.DateAttest;
            DateMed = p.DateMed;
        }
        public PersonList(string fullName, string position, string? dateAttest, string? dateMed)
        {
            FullName = fullName;
            Position = position;
            DateAttest = dateAttest;
            DateMed = dateMed;
        }

        public PersonList()
        {
        }

        public string? FullName { get; set; }
        public string? Position { get; set; }
        
        public string? DateAttest { get; set; }
      
        public string? DateMed { get; set; }
    }
}