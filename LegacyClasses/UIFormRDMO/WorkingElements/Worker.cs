using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UIFormRDMO.Data;
using UIFormRDMO.Data.Models;

namespace UIFormRDMO.WorkingElements
{
    public static class Worker
    {
        private static PersonsContext _context;

        /// <summary>
        /// Есть в штатке, но не в списках от инструкторов
        /// </summary>
        private static List<IPerson> _outOfDB = new List<IPerson>();

        /// <summary>
        /// Есть в списках от инструкторов, но нет в штатке
        /// </summary>
        private static List<IPerson> _outOfRDMO = new List<IPerson>();
        
        /// <summary>
        /// Есть в списках от инструкторов, но нет в штатке
        /// </summary>
        public static List<IPerson> OutOfRDMO
        {
            get
            {
                return new List<IPerson>(_outOfRDMO);
            }
            set
            {
                _outOfRDMO = new List<IPerson>(value);
            }
        }
        
        /// <summary>
        /// Есть в штатке, но не в списках от инструкторов
        /// </summary>
        public static List<IPerson> OutOfDB
        {
            get
            {
                return new List<IPerson>(_outOfDB);
            }
            set
            {
                _outOfDB = new List<IPerson>(value);
            }
        }

        public static void Compare(PersonsContext context)
        {
            _context = context;
            // Формируем список тех, кого потеряли (есть в штатке, но нет в списках)
            FillOutOfDB();

            // Формируем список тех, кого потенциально уже уволили (есть в списоках от МИ, но нет в штатке)
            FillOutOfRDMO();
        }

        /// <summary>
        /// Формируем список тех, кого потеряли (есть в штатке, но нет в списках)
        /// </summary>
        private static void FillOutOfDB()
        {
            _context.PersonDbs.ForEach(e =>
            {
                if (!_context.PersonLists.SoftContains(e))
                {
                    _outOfDB.Add(new PersonList(e));
                }
            });
        }

        /// <summary>
        /// Формируем список тех, кого потенциально уволили
        /// </summary>
        private static void FillOutOfRDMO()
        {
            _context.PersonLists.ForEach(e =>
            {
                if (!_context.PersonDbs.SoftContains(e))
                {
                    _outOfRDMO.Add(new PersonList(e));
                }
            });
        }

        private static bool SoftContains(this IEnumerable<IPerson> lst, IPerson obj)
        {
            bool result = false;
            lst.ToList().ForEach(e =>
            {
                string softNameInList = e.FullName!.Replace('ё', 'е').ToString();
                string softNameInObj = obj.FullName!.Replace('ё', 'е');
                
                if (softNameInList.ToString(CultureInfo.InvariantCulture) == softNameInObj.ToString(CultureInfo.InvariantCulture))
                {
                    result = true;
                }
            });
            return result;
        }

        /// <summary>
        /// Вне зависимости от сравнения таблиц - подготавливает результирующую и записывает в файл
        /// </summary>
        public static void PrepareResultTable(bool writeToFile = false)
        {
            //_outOfDB.Clear(); FillOutOfDB();
            
            _context.PersonLists.ForEach(e =>
            {
                PersonList person = new PersonList(e);
                IPerson findPerson = _context.PersonDbs.CustomFind(e);
                
                // Если не нашли такого в штатке, то просто переносим из списка
                if ((findPerson as ErrorObject) != null)
                {
                    _context.ResultList.Add(person);
                }
                else
                {
                    // иначе - обновляем данные (сейвово) и заносим в резалт
                    PrepareDates(ref person, findPerson);
                    _context.ResultList.Add(person);
                }
            });
            
            // дозаносим остатки
            _outOfDB.ForEach(e =>
            {
                PersonList person = new PersonList(e);
                _context.ResultList.Add(person);
            });
        }

        private static void PrepareDates(ref PersonList person, IPerson findPerson)
        {
            if (!String.IsNullOrEmpty(findPerson.DateAttest))
            {
                person.DateAttest = findPerson.DateAttest;
            }

            if (!String.IsNullOrEmpty(findPerson.DateMed))
            {
                person.DateMed = findPerson.DateMed;
            }
            
            if (!String.IsNullOrEmpty(findPerson.Position))
            {
                person.Position = findPerson.Position;
            }
        }

        private static IPerson CustomFind(this IEnumerable<IPerson> lst, IPerson person)
        {
            var list = lst as List<IPerson>;
            for (int i = 0; i < list.Count(); i++)
            {
                string softNameInList = list[i].FullName!.Replace('ё', 'е').ToString();
                string softNameInObj = person.FullName!.Replace('ё', 'е');
                if (softNameInList == softNameInObj)
                {
                    return new PersonList(list[i]);
                }
            }

            return new ErrorObject();
        }
    }
}