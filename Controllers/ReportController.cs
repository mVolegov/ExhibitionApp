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
            var tables = new List<Table>() { new Table { Name = "Тип выставки" },
                new Table { Name = "Города"},
                new Table { Name = "Улицы"},
                new Table { Name = "Страны"},
                new Table { Name = "Пол"},
            };

            ViewData["ImportTables"] = new SelectList(tables, "Name", "Name", "Тип выставки");
            ViewData["ExportTables"] = new SelectList(tables, "Name", "Name", "Тип выставки");
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(bool isRewriteImport, string importTable, IFormFile importFile)
        {
            if (importFile == null || Path.GetExtension(importFile.FileName) != ".csv")
            {
                return RedirectToAction("Index", new { errorMessage = "Неверное расширение файла!" });
            }
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
                    case "Тип выставки":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE \"ExhibitTypes\" RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("Index", new { errorMessage = "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи" });
                            }
                        }
                        var recordsNomenclature = csv.GetRecords<ExhibitType>().ToList();
                        _db.ExhibitTypes.AddRange(recordsNomenclature);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('ExhibitTypes_Id_seq', (SELECT MAX(id) from \"ExhibitTypes\"));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Города":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE \"Cities\" RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("Index", new { errorMessage = "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи" });
                            }
                        }
                        var recordsProperty = csv.GetRecords<City>().ToList();
                        _db.Cities.AddRange(recordsProperty);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('Cities_Id_seq', (SELECT MAX(id) from \"Cities\"));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Улицы":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE \"Streets\" RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("Index", new { errorMessage = "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи" });
                            }
                        }
                        var recordsCustomer = csv.GetRecords<Street>().ToList();
                        _db.Streets.AddRange(recordsCustomer);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('Streets_Id_seq', (SELECT MAX(id) from \"Streets\"));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Страны":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE \"Countries\" RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("Index", new { errorMessage = "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи" });
                            }
                        }
                        var recordsSupplier = csv.GetRecords<Country>().ToList();
                        _db.Countries.AddRange(recordsSupplier);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('Countries_Id_seq', (SELECT MAX(id) from \"Countries\"));");
                        await _db.SaveChangesAsync();
                        break;

                    case "Пол":
                        if (isRewriteImport)
                        {
                            try
                            {
                                _db.Database.ExecuteSqlRaw("TRUNCATE \"Sexes\" RESTART IDENTITY;");
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("Index", new { errorMessage = "Невозможно перезаписать данные, на таблицу ссылаются внешние ключи" });
                            }
                        }
                        var recordsWarehouse = csv.GetRecords<Sex>().ToList();
                        _db.Sexes.AddRange(recordsWarehouse);
                        await _db.SaveChangesAsync();
                        _db.Database.ExecuteSqlRaw("SELECT setval('Sexes_Id_seq', (SELECT MAX(id) from \"Sexes\"));");
                        await _db.SaveChangesAsync();
                        break;
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", new { errorMessage = "Неверный формат данных в файле, импорт не удался" });
            }

            return RedirectToAction("Index");
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
                case "Типы выставок":
                    await csv.WriteRecordsAsync(_db.ExhibitTypes.ToList());
                    tableName = "ExhibitTypes";
                    break;

                case "Города":
                    await csv.WriteRecordsAsync(_db.Cities.ToList());
                    tableName = "Cities";
                    break;

                case "Улицы":
                    await csv.WriteRecordsAsync(_db.Streets.ToList());
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

            sheet.Protection.IsProtected = true;

            string path = "/files/productReport.xlsx";
            System.IO.File.WriteAllBytes(_appEnvironment.WebRootPath + path, package.GetAsByteArray());

            return File(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Отчет об экспонатах.xlsx");
        }
    }
}