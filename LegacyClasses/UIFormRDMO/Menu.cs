using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using UIFormRDMO.Data;
using UIFormRDMO.Data.Models;
using UIFormRDMO.Enums;
using UIFormRDMO.ExcelWork;
using UIFormRDMO.WorkingElements;

namespace UIFormRDMO
{
    public static class Menu
    {
        internal static PersonsContext _context = new PersonsContext();
        internal static BaseHelper helper = new BaseHelper(_context);

        public static int SelectOption(bool skipMenu = false)
        {
            int v = -1;
            StringBuilder sb = new StringBuilder();

            if (!skipMenu)
            {
                sb.AppendLine("0.ВЫХОД");
                sb.AppendLine("1.Указать путь до файла с данными");
                sb.AppendLine("2.Показать базу РДМО");
                sb.AppendLine("3.Показать базу ШТАТ");
                sb.AppendLine("4.Заполнить данные");
                sb.AppendLine("5.Очистить ВСЕ базы");
                sb.AppendLine("6.Начать процедуру сравнения");
            }

            Console.Write(sb + "Введите число-вариант >");
            try
            {
                v = int.Parse(Console.ReadLine()!);
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так! Попробуйте ещё раз)");
                v = SelectOption(true);
            }

            return v;
        }

        private static string? _defaultPath = $"C:/Users/{Environment.UserName}/Desktop/data.txt";
        private static string? _path;

        public static string? Path
        {
            get
            {
                if (_path is null or "")
                {
                    return _defaultPath;
                }

                return _path;
            }
            set
            {
                if (value is null or "")
                {
                    _path = _defaultPath;
                }
                else
                {
                    _path = value;
                }
            }
        }
        
        public static string StartWork(string[] variants)
        {

            string message = "";
            switch (int.Parse(variants[0]))
            {
                case 0:
                {
                    break;
                }
                case 1:
                {
                    Console.WriteLine(
                        "Введите полный путь до файла с названием и его раширением, пример: C:/System/file.txt");
                    Console.WriteLine(
                        "!!!Напоминание - в файле сперва идёт список от МИ, потом == и уже после - штатка!!!");
                    Console.Write($"Текущий путь до файла: {Path} \nНовый путь(введите 0, чтобы отменить изменения)>");
                    //string input = Console.ReadLine();
                    // if (input.Equals("0"))
                    // {
                    //     break;
                    // }
                    
                    Path = variants[1];
                    break;
                }
                case 2:
                {
                    // вывод РДМО
                    var data = new OutputInformationHelper(_context).GetAllDatabaseByTable(Table.PersonsList);
                    Console.WriteLine(data);
                    break;
                }
                case 3:
                {
                    // вывод ШТАТ
                    var data = new OutputInformationHelper(_context).GetAllDatabaseByTable(Table.PersonsDB);
                    Console.WriteLine(data);
                    break;
                }
                case 4:
                {
                    // Заполнение данных
                    try
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        using (StreamReader reader = new StreamReader(Path))
                        {
                            stringBuilder.Append(reader.ReadToEnd());
                            reader.Close();
                        }

                        var input = stringBuilder.Replace("\r", "").ToString();

                        helper.InjectInDB(Table.PersonsList,
                            OutputInformationHelper.GetListsByString(input).firstTable
                                .Split('\n').ToList());
                        helper.InjectInDB(Table.PersonsDB,
                            OutputInformationHelper.GetListsByString(input).secondTable
                                .Split('\n').ToList());
                    }
                    catch (Exception e)
                    {
                        return $"При заполнении данных что-то пошло не так! Проверьте путь и повторите попытку!";
                    }

                    break;
                }
                case 5:
                {
                    // Очистить списки
                    helper.ClearDB();
                    message = "Вся база очищена";
                    break;
                }
                case 6:
                {
                    // Процедура сравнения
                    try
                    {
                        Worker.Compare(_context);
                        Worker.PrepareResultTable();
                        //helper.WriteToFile("res", Path);

                        var package = new ExcelPackage();
                        package.GenerateListInFile(_context.ResultList, "Результирующий список");
                        package.GenerateListInFile(Worker.OutOfDB, "Нет в списках МИ");
                        package.GenerateListInFile(Worker.OutOfRDMO, "Нет в списках штат");
                        File.WriteAllBytes(helper.GetFullPathForExel(Path, "res"), package.GetAsByteArray());
                    }
                    catch (Exception e)
                    {
                        return $"При сравнении что-то пошло не так! Проверьте путь и повторите попытку! " +
                               $"{e.ToErrorObject().ErrorCode}";
                    }
                    break;
                }
                default:
                {
                    Console.WriteLine("Для выхода введите 0");
                    break;
                }
            }

            return $"ok {message}";
        }
    }
}