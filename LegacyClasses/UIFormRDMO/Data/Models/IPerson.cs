
namespace UIFormRDMO.Data.Models
{
    public interface IPerson
    {
        
        public string? FullName { get; set; }
        public string? Position { get; set; }
       
        public string? DateAttest { get; set; }
      
        public string? DateMed { get; set; }
    }
}