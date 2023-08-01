using System;
using System.Collections.Generic;
using System.Text;
using UIFormRDMO.Data.Models;
using UIFormRDMO.Enums;

namespace UIFormRDMO.WorkingElements
{
    public class OutputInformationHelper
    {
        private PersonsContext _context;
        public OutputInformationHelper(PersonsContext context)
        {
            _context = context;
        }

        public string GetAllDatabaseByTable(Table table)
        {
            StringBuilder sb = new StringBuilder();
            MapTab(table)?.ForEach(e =>
            {
                sb.AppendLine($"{e.FullName};{e.Position};{e.DateAttest ?? "null"};{e.DateMed ?? "null"}");
            });
            return sb.ToString();
        }

        private List<IPerson> MapTab(Table table)
        {
            if (table == Table.PersonsList)
            {
                return _context.PersonLists;
            }

            return _context.PersonDbs;
        }
        
        public static (string firstTable, string secondTable) GetListsByString(string arg)
        {
            var t = arg;
            var index = t.IndexOf("=", StringComparison.Ordinal);
            var firstTable = t.Substring(0, index);
            var secondTable = t.Substring(index + 3);
            
            return (firstTable, secondTable);
        }
    }
}