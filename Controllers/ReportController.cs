using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TemplateEngine;
using System.Globalization;
using ExhibitionApp.Data;
using ExhibitionApp.Models;
using ExhibitionApp.Utils;
using TemplateEngine.Docx;
using Microsoft.AspNetCore.Authorization;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ExhibitionAppDbContext _db;
        private readonly IWebHostEnvironment _appEnvironment;

        public ReportController(ILogger<ReportController> logger, ExhibitionAppDbContext context, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _db = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index(string? errorMessage)
        {
            var tables = new List<Table>() { 
                new Table { Name = "Тип экспоната" },
                new Table { Name = "Города"},
                new Table { Name = "Улицы"},
                new Table { Name = "Страны"},
                new Table { Name = "Пол"},
            };

            ViewData["ImportTables"] = new SelectList(tables, "Name", "Name", "Тип экспоната");
            ViewData["ExportTables"] = new SelectList(tables, "Name", "Name", "Тип экспоната");
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(string importTable, IFormFile importFile)
        {
            if (importFile == null || Path.GetExtension(importFile.FileName) != ".csv")
            {
                return RedirectToAction("Index", new { errorMessage = "Неверное расширение файла!" });
            }

            // Форматирование заголовков файла (Id, Name и т.д.)
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", ""),
            };
            using var reader = new StreamReader(importFile.OpenReadStream());
            using var csv = new CsvReader(reader, config);

            try
            {
                switch (importTable)
                {
                    case "Тип экспоната":
                        var recordsExhibitTypes = csv.GetRecords<ExhibitType>().ToList();

                        foreach (ExhibitType record in recordsExhibitTypes) 
                        {
                            if (!_db.ExhibitTypes.Contains(record)) _db.ExhibitTypes.Add(record);
                        }

                        await _db.SaveChangesAsync();
                        
                        break;

                    case "Города":
                        var recordsCities = csv.GetRecords<City>().ToList();

                        foreach (var record in recordsCities)
                        {
                            if (!_db.Cities.Contains(record)) _db.Cities.Add(record);
                        }

                        await _db.SaveChangesAsync();

                        break;

                    case "Улицы":
                        var recordsStreets = csv.GetRecords<Street>().ToList();

                        foreach (var record in recordsStreets)
                        {
                            record.City = _db.Cities.FirstOrDefault(c => c.Id == record.CityId);
                            if (!_db.Streets.Contains(record)) _db.Streets.Add(record);
                        }

                        await _db.SaveChangesAsync();

                        break;

                    case "Страны":
                        var recordsCountry = csv.GetRecords<Country>().ToList();

                        foreach (var record in recordsCountry)
                        {
                            if (!_db.Countries.Contains(record)) _db.Countries.Add(record);
                        }

                        await _db.SaveChangesAsync();

                        break;

                    case "Пол":
                        var recordsSexes = csv.GetRecords<Sex>().ToList();

                        foreach (var record in recordsSexes)
                        {
                            if (!_db.Sexes.Contains(record)) _db.Sexes.Add(record);
                        }

                        await _db.SaveChangesAsync();

                        break;
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { errorMessage = "Неверный формат данных в файле, импорт не удался" });
            }

            return RedirectToAction("Index", new { errorMessage = "Успешно импортировано" });
        }

        [HttpPost]
        public async Task<IActionResult> Export(string exportTable)
        {
            string path = "/files/export.csv";
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower().Replace("_", "")
            };
            var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create, FileAccess.ReadWrite);
            using var writer = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            using var csv = new CsvWriter(writer, config);
            var tableName = string.Empty;

            switch (exportTable)
            {
                case "Тип экспоната":
                    await csv.WriteRecordsAsync(_db.ExhibitTypes.ToList());
                    tableName = "ExhibitTypes";
                    break;

                case "Города":
                    await csv.WriteRecordsAsync(_db.Cities.ToList());
                    tableName = "Cities";
                    break;

                case "Улицы":
                    await csv.WriteRecordsAsync(_db.Streets.Include(s => s.City).ToList());
                    tableName = "Streets";
                    break;

                case "Страны":
                    await csv.WriteRecordsAsync(_db.Countries.ToList());
                    tableName = "Countries";
                    break;

                case "Пол":
                    await csv.WriteRecordsAsync(_db.Sexes.ToList());
                    tableName = "Sexes";
                    break;
            }            

            return File(path, "text/csv", $"exported_table_{tableName}.csv");
        }

        [HttpPost]
        public IActionResult ExhibitsReport(DateTime? dateStartArrived, DateTime? dateEndArrived)
        {
            var sampleReportPath = "/files/exhibitReport.docx";
            var reportPath = "/files/exhibitReportFilled.docx";

            try { System.IO.File.Delete(_appEnvironment.WebRootPath + reportPath); }
            catch { }

            System.IO.File.Copy(_appEnvironment.WebRootPath + sampleReportPath, _appEnvironment.WebRootPath + reportPath);

            using var outputDocument = new TemplateProcessor(_appEnvironment.WebRootPath + reportPath).SetRemoveContentControls(true);

            if (dateStartArrived == null) dateStartArrived = DateTime.Now;
            if (dateEndArrived == null) dateEndArrived = DateTime.Now;

            var valuesToFillNew = new Content();
            var valuesToFill = valuesToFillNew.Append(new FieldContent("Date", DateOnly.FromDateTime(DateTime.Now).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateStart", DateOnly.FromDateTime(dateStartArrived.Value).ToString()));
            valuesToFill = valuesToFill.Append(new FieldContent("DateEnd", DateOnly.FromDateTime(dateEndArrived.Value).ToString()));

            var queryExhibits = _db.Exhibits
                .Include(p => p.Warehouse)
                .Include(p => p.Authors)
                .Include(p => p.Exhibitions)
                .Include(p => p.ExhibitType)
                .Include(p => p.Warehouse.Address)
                .Include(p => p.Warehouse.Address.Street)
                .Where(ps => 
                    ps.ArrivalDate >= DateOnly.FromDateTime(dateStartArrived.Value) && 
                    ps.ArrivalDate <= DateOnly.FromDateTime(dateEndArrived.Value))
                .OrderBy(ps => ps.ArrivalDate);

            List<TableRowContent> rows = new();
            foreach (var exhibit in queryExhibits.ToList())
            {
                rows.Add(new TableRowContent(new FieldContent("ExhibitId", exhibit.Id.ToString()),
                new FieldContent("ExhibitName", exhibit.Name),
                new FieldContent("ExhibitAddress", exhibit.Warehouse.Address.Street.Name + " " + exhibit.Warehouse.Address.HouseNumber),
                new FieldContent("ExhibitArrivalDate", exhibit.ArrivalDate.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("AllExhibits", rows));

            var exhibitsInWarehouses = queryExhibits.GroupBy(ps => ps.Warehouse).Select(g => new
            {
                g.Key,
                Count = g.Count()
            });
            rows = new();
            foreach (var warehouse in exhibitsInWarehouses)
            {
                rows.Add(new TableRowContent(new FieldContent("WarehouseAddress", warehouse.Key.Address.Street.Name + " " + warehouse.Key.Address.HouseNumber),
                new FieldContent("NomenclatureTypeWorth", warehouse.Count.ToString())));
            }
            valuesToFill = valuesToFill.Append(new TableContent("WarehouseExhibits", rows));

            valuesToFill = valuesToFill.Append(new FieldContent("ExhibitCount", queryExhibits.Count().ToString()));
            outputDocument.FillContent(new Content(valuesToFill.ToArray()));
            outputDocument.SaveChanges();

            return File(reportPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет об экспонатах (от {DateOnly.FromDateTime(DateTime.Now)}).docx");
        }

        [HttpPost]
        public IActionResult ExcelReport()
        {
            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Отчет по экспонатам");

            sheet.Cells["B2"].Value = "Отчет по экспонатам";
            sheet.Cells["B2"].Style.Font.Bold = true;
            sheet.Cells["B3"].Value = "Дата = ";
            sheet.Cells["B3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells["C3"].Value = DateOnly.FromDateTime(DateTime.Now);

            sheet.Cells["B4:I4"].LoadFromArrays(new object[][] { new[] { "ID экспоната", "Наименование", "Дата создания", "Дата поступления", "Тип", "Адрес", "Кол-во авторов", "Кол-во появлений на выставках" } });

            var list = _db.Exhibits.Include(p => p.Warehouse).Include(p => p.Authors).Include(p => p.Exhibitions).Include(p => p.ExhibitType).Include(p => p.Warehouse.Address).Include(p => p.Warehouse.Address.Street);
            var row = 5;
            var column = 2;
            foreach (var item in list)
            {
                sheet.Cells[row, column].Value = item.Id;
                sheet.Cells[row, column + 1].Value = item.Name;
                sheet.Cells[row, column + 2].Value = item.CreationDate;
                sheet.Cells[row, column + 3].Value = item.ArrivalDate;
                sheet.Cells[row, column + 4].Value = item.ExhibitType.TypeName;
                sheet.Cells[row, column + 5].Value = item.Warehouse.Address.Street.Name + " " + item.Warehouse.Address.HouseNumber;
                sheet.Cells[row, column + 6].Value = item.Authors.Count;
                sheet.Cells[row, column + 7].Value = item.Exhibitions == null ? "0" : item.Exhibitions.Count;
                row++;
            }

            sheet.Cells[1, 1, row, column + 9].AutoFitColumns();
            sheet.Cells[4, 2, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            sheet.Cells[row, column + 6].Value = "Итого:";
            sheet.Cells[5, column + 7, row - 1, column + 7].Style.Numberformat.Format = "#";
            sheet.Cells[row, column + 7].Formula = $"SUM({(char)('A' + column + 6)}{5}:{(char)('A' + column + 6)}{row - 1})";
            sheet.Cells[row, column + 7].Calculate();
            sheet.Cells[row, column + 8].Value = "Кол-во экспонатов:";
            sheet.Cells[row, column + 9].Value = _db.Exhibits.Count();
            sheet.Cells[row, column + 6, row, column + 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
            sheet.Cells[row, column + 8].AutoFitColumns();

            sheet.Protection.IsProtected = false;

            string path = "/files/productReport.xlsx";
            System.IO.File.WriteAllBytes(_appEnvironment.WebRootPath + path, package.GetAsByteArray());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчет об экспонатах.xlsx");
        }
    }
}