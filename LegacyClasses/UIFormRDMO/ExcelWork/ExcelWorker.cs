using System;
using System.Collections.Generic;
using OfficeOpenXml;
using UIFormRDMO.Data.Models;

namespace UIFormRDMO.ExcelWork;

public static class ExcelWorker
{
    public static ExcelPackage GenerateListInFile(this ExcelPackage package, List<IPerson> resultList, string name)
    {
        try
        {
            var sheetResult = package.Workbook.Worksheets.Add(name);
            sheetResult.Cells["A1"].Value = "ФИО";
            sheetResult.Cells["B1"].Value = "Должность";
            sheetResult.Cells["C1"].Value = "Дата аттестации";
            sheetResult.Cells["D1"].Value = "Дата МЕД";

            var row = 2;
            var column = 1;
            resultList.ForEach(person =>
            {
                sheetResult.Cells[row, column].Value = person.FullName;
                sheetResult.Cells[row, column + 1].Value = person.Position;
                sheetResult.Cells[row, column + 2].Value = person.DateAttest;
                sheetResult.Cells[row, column + 3].Value = person.DateMed;
                row++;
            });

            return package;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}