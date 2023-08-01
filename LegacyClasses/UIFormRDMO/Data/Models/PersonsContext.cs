using System.Collections.Generic;

namespace UIFormRDMO.Data.Models
{
    public class PersonsContext
    {
        public PersonsContext()
        {
            PersonLists = new List<IPerson>();
            PersonDbs = new List<IPerson>();
            ResultList = new List<IPerson>();
        }
        
        /// <summary>
        /// Результирующий лист
        /// </summary>
        public List<IPerson> ResultList { get; set; }

        /// <summary>
        /// Списки инструкторов
        /// </summary>
        public List<IPerson> PersonLists { get; set; }
        
        /// <summary>
        /// Списки ШТАТ
        /// </summary>
        public List<IPerson> PersonDbs { get; set; }
    }
}