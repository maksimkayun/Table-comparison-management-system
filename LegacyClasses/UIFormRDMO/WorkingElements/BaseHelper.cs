using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UIFormRDMO.Data.Models;
using UIFormRDMO.Enums;

namespace UIFormRDMO.WorkingElements
{
    public class BaseHelper
    {
        private PersonsContext _context;

        public BaseHelper(PersonsContext context)
        {
            _context = context;
        }
        internal void ClearDB()
        {
            _context.PersonLists.Clear();
            _context.PersonDbs.Clear();
            _context.ResultList.Clear();
        }

        internal List<Person> ConvertListInPersons(Table table, List<string> list)
        {
            List<Person> persons = new List<Person>();
            // Если это из списка инструкторов, то даты не учитываем
            try
            {
                if (table == Table.PersonsList)
                {
                    list.ForEach(e =>
                    {
                        var person = new Person();
                        person.FullName = e.Split(';')[0].ToString(CultureInfo.InvariantCulture);
                        person.Position = e.Split(';')[1].ToString(CultureInfo.InvariantCulture);
                        if (e.Split(';').Length > 2)
                        {
                            person.DateAttest = e.Split(';')[2] == "" ? null : e.Split(';')[2];
                            person.DateMed = e.Split(';')[3] == "" ? null : e.Split(';')[3];
                        }

                        persons.Add(person);
                    });
                }
                else
                {
                    list.ForEach(e =>
                    {
                        var person = new Person();
                        person.FullName = e.Split(';')[0].ToString(CultureInfo.InvariantCulture);
                        person.Position = e.Split(';')[1].ToString(CultureInfo.InvariantCulture);
                        person.DateAttest = e.Split(';')[2].ToString(CultureInfo.InvariantCulture);
                        person.DateMed = e.Split(';')[3].ToString(CultureInfo.InvariantCulture);
                        persons.Add(person);
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return persons;
        }

        public class Person
        {
            public string FullName { get; set; }
            public string Position { get; set; }
            public string DateAttest { get; set; }
            public string DateMed { get; set; }
        }


        private Person SoftConvert(IPerson personDb)
        {
            Person person = new Person();
            person.FullName = personDb.FullName;
            person.Position = personDb.Position;
            person.DateAttest = "null";
            person.DateMed = "null";
            return person;
        }

        private Person Convert(IPerson arg)
        {
            Person person = new Person();
            person.FullName = arg.FullName;
            person.Position = arg.Position;
            person.DateAttest = arg.DateAttest ?? "null";
            person.DateMed = arg.DateMed ?? "null";
            return person;
        }


        internal void InjectInDB(Table table, List<string> list)
        {
            list.Remove("");
            var data = ConvertListInPersons(table, list);
            if (table == Table.PersonsList)
            {
                data.ForEach(e =>
                {
                    _context.PersonLists.Add(new PersonList
                    {
                        FullName = e.FullName,
                        Position = e.Position,
                        DateAttest = e.DateAttest,
                        DateMed = e.DateMed
                    });
                });
            }
            else
            {
                data.ForEach(e =>
                {
                    _context.PersonDbs.Add(new PersonDB
                    {
                        FullName = e.FullName,
                        DateAttest = e.DateAttest,
                        DateMed = e.DateMed,
                        Position = e.Position
                    });
                });
            }
        }

        public string GetFullPathForExel(string? path, string filename)
        {
            var prepareToDelete = path?.Split('/').LastOrDefault();
            path = path?.Replace(prepareToDelete, "");
            
            return $@"{path}{filename}.xlsx";
        }
        
        [Obsolete("Используйте ExcelPackage")]
        public void WriteToFile(string filename, string? path)
        {
            var prepareToDelete = path?.Split('/').LastOrDefault();
            path = path?.Replace(prepareToDelete, "");
            
            string fullpath = $@"{path}{filename}.txt";
            using (StreamWriter sw = new StreamWriter(fullpath))
            {
                sw.Write(CustomToString());
                sw.Close();
            }
        }

        public string CustomToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ГОТОВЫЙ СПИСОК");
            _context.ResultList.ForEach(e =>
            {
                sb.AppendLine($"{e.FullName};{e.Position};{e.DateAttest};{e.DateMed}");
            });
            
            sb.AppendLine("\n============\nСПИСОК ТЕХ, КОГО НЕ НАШЛИ В СПИСКАХ ОТ МИ");
            Worker.OutOfDB.ForEach(e =>
            {
                sb.AppendLine($"{e.FullName};{e.Position};{e.DateAttest};{e.DateMed}");
            });
            
            sb.AppendLine("\n============\nСПИСОК ТЕХ, КОГО НЕТ ШТАТКЕ, НО ОНИ ЕСТЬ В СПИСКАХ ОТ МИ");
            Worker.OutOfRDMO.ForEach(e =>
            {
                sb.AppendLine($"{e.FullName};{e.Position};{e.DateAttest};{e.DateMed}");
            });
            return sb.ToString();
        }
    }
}