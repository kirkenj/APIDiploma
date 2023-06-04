using ClosedXML.Excel;
using Database.Entities;
using Database.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Logic.Services
{
    public class ExcelReportService : IExcelReportService
    {
        #region headersColumns
        private readonly string ListNumberColumnLetter = "A";
        private readonly string NSPColumnLetterStart = "B";
        private readonly string NSPColumnLetterEnd = "D";
        private readonly string DegreeColumnLetter = "E";
        private readonly string ContractColumnLetterStart = "F";
        private readonly string ContractColumnLetterEnd = "G";
        private readonly string HomeAddressAndPassportColumnLetterStart = "H";
        private readonly string HomeAddressAndPassportColumnLetterEnd = "K";
        private readonly string AllowedHoursColumnLetter = "L";
        private readonly string PrevDoneHoursColumnLetter = "M";
        private readonly string HourlyPaidWorkHeaderColumnLetterStart = "L";
        private readonly string HourlyPaidWorkHeaderColumnLetterEnd = "S";
        private readonly string ToBeCalcucatedByAccountingColumnLetterStart = "N";
        private readonly string ToBeCalcucatedByAccountingColumnLetterEnd = "S";
        private readonly string LeftoverColumnLetter = "N";
        private readonly string HoursAmmColumnLetter = "O";
        private readonly string HourPriceColumnLetter = "P";
        private readonly string SumToBePadColumnLetterStart = "Q";
        private readonly string SumToBePadColumnLetterEnd = "R";
        private readonly string ProffesorSignColumnLetter = "S";
        private readonly string ZaPageAddres = "B7";
        private readonly int headerStart = 9;
        private readonly int headerEnd = 11;
        #endregion

        private readonly AcademicDegree DefaultDegree = new AcademicDegree
        {
            ID = -1,
            Name = "  ",
        };
        
        private readonly DbSet<MonthReport> _monthReportDbSet;
        private readonly DbSet<ContractLinkingPart> _contractLinkingPartDbSet;
        private readonly IAcademicDegreeService _academicDegreeService;
        private readonly IDepartmentService _departmentService;
        private readonly IAccountService _accountService;
        private readonly IContractTypeService _contractTypeService;

        private readonly string[] MonthsNames = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        private readonly string[] Headers = new string[] { "Лекции", "Практические занятия", "Лабораторные занятия", "Консультации", "Иное учебное занятие", "Зачеты", "Экзамены", "Курсовая работа, курсовой проект", "Собеседование ", "Контрольная работа, реферат", "Стажировка", "Дипломный проект, дипломная работа", "Рецензирование дипломных проектов, работ, маг.диссертаций", "ГЭК", "Руководство магистрантами", "Учебная работа с аспирантами", "Днмонстрация пластических поз", "Сопровождение тестирования", "Итого часов" };

        public ExcelReportService(IAppDBContext appDBContext, IAccountService accountService, IAcademicDegreeService academicDegreeService, IDepartmentService departmentService, IContractTypeService contractTypeService)
        {
            _monthReportDbSet = appDBContext.Set<MonthReport>();
            _academicDegreeService = academicDegreeService;
            _departmentService = departmentService;
            _contractTypeService = contractTypeService;
            _academicDegreeService = academicDegreeService;
            _accountService = accountService;
            _contractLinkingPartDbSet = appDBContext.Set<ContractLinkingPart>();
        }


        public async Task<string> GetReport(DateTime dateStart, DateTime dateEnd, IEnumerable<int>? reqDepartmentIDs = null)
        {
            if (dateStart > dateEnd)
            {
                (dateStart, dateEnd) = (dateEnd, dateStart);
            }

            var path = $"D:\\{Guid.NewGuid()}.xlsx";

            int firstColumnIndex = 21;

            using (var workbook = new XLWorkbook())
            {
                var reportsRequest = _monthReportDbSet.Include(m => m.LinkingPart).ThenInclude(l => l.Assignments).ThenInclude(c => c.User)
                    .Where(r => r.MontYearAsDate >= dateStart && r.MontYearAsDate <= dateEnd);



                if (reqDepartmentIDs != null && reqDepartmentIDs.Any())
                {
                    reportsRequest = reportsRequest.Where(m => reqDepartmentIDs.Contains(m.LinkingPart.Assignments.First().DepartmentID));
                }

                reportsRequest = reportsRequest.OrderBy(r => r.MontYearAsDate);
                var recordsList = await reportsRequest.ToListAsync();
                                
                if (!recordsList.Any())
                {
                    throw new ArgumentException("No records found with given parameters");
                }

                var datesOnPeriod = DateTimeProvider.GetDateRangeViaAddMonth(dateStart, dateEnd);




                var linkingPartsList = await _contractLinkingPartDbSet.Include(l => l.Assignments).Include(l => l.MonthReports).Where(c => reportsRequest.Select(r => r.LinkingPartID).Distinct().Contains(c.ID)).ToListAsync();
                var contractTypesList = await _contractTypeService.GetListViaSelectionObjectAsync(new Models.ContractType.ContractTypesSelectObject { IDs = linkingPartsList.Select(l => l.Assignments.First().ContractTypeID).Distinct() });
                var departmentsList = await _departmentService.GetListViaSelectionObjectAsync(new Models.Department.DepartmentSelectObject { IDs = linkingPartsList.Select(l => l.Assignments.First().DepartmentID).Distinct() });
                var degreesList = await _academicDegreeService.GetListViaSelectionObjectAsync(null);


                Dictionary<int, (int LinkingPartID, double ReportsTimeSumValue, string formula, bool IsRef)> linkimgPartDict = linkingPartsList.Select(l => (LinkingPartID:l.ID, ReportsTimeSumValue:l.MonthReports.Where(r => r.MontYearAsDate < dateStart).Sum(r => r.TimeSum), formula:string.Empty, IsRef:false)).ToDictionary(i => i.LinkingPartID);


                var grByDep = recordsList.GroupBy(r => r.LinkingPart.Assignments.First().DepartmentID);


                var depsSheet = workbook.Worksheets.Add($"Отделы");
                var depsExcelDict = AddDepartmentsOnPage(depsSheet, departmentsList);
                var degreesSheet = workbook.Worksheets.Add($"Академ.степени");
                var degreesDict = await AddAcademicDegreesWithAssignments(degreesSheet, degreesList, datesOnPeriod);

                var typeSheet = workbook.Worksheets.Add("Типы_договоров");
                var ctypesDict = await AddContractTypesWithAssignments(typeSheet, contractTypesList, datesOnPeriod);


                foreach (var grDep in grByDep)
                {

                    var grByCType = grDep.GroupBy(r => r.LinkingPart.Assignments.First().ContractTypeID);
                    var curDep = departmentsList.First(c => c.ID == grDep.Key);
                    var userSheet = workbook.Worksheets.Add($"{curDep.Name}_работники");
                    var usersExceDict = await AddUsersOnPage(userSheet, grDep.OrderBy(r => r.LinkingPart.Assignments.First().ContractTypeID).Select(r => r.LinkingPart.Assignments.First().User).Distinct(), degreesDict, datesOnPeriod);



                    var technoSheet = workbook.Worksheets.Add($"{curDep.Name}_Константы");
                    Dictionary<string, string> positionsPeopleRefsDict = new();
                    string key = "Проректор по учебной работе";
                    technoSheet.Cell("A1").Value = key;
                    technoSheet.Cell("B1").Value = "А.П.КУРАКОВА";
                    positionsPeopleRefsDict.Add(key, $"{technoSheet.Name}!B1");

                    key = "Зам.начальника ПЭУ";
                    technoSheet.Cell("A2").Value = key;
                    technoSheet.Cell("B2").Value = "Н.К.Зновец";
                    positionsPeopleRefsDict.Add(key, $"{technoSheet.Name}!B2");
                    key = "Начальник УМУ";
                    technoSheet.Cell("A3").Value = key;
                    technoSheet.Cell("B3").Value = "Л.И.Шахрай";
                    positionsPeopleRefsDict.Add(key, $"{technoSheet.Name}!B3");
                    key = "Декан факультета";
                    technoSheet.Cell("A4").Value = key;
                    technoSheet.Cell("B4").Value = "А.М.Авсиевич";
                    positionsPeopleRefsDict.Add(key, $"{technoSheet.Name}!B4");
                    key = "Заведующий кафедрой";
                    technoSheet.Cell("A5").Value = key;
                    technoSheet.Cell("B5").Value = "Ю.В.Полозков";
                    positionsPeopleRefsDict.Add(key, $"{technoSheet.Name}!B5");


                    foreach (var grType in grByCType)
                    {
                        var grByMonth = grType.GroupBy(r => r.MontYearAsDate);
                        var curType = contractTypesList.First(c => c.ID == grType.Key);
                        foreach (var grMonth in grByMonth)
                        {
                            var sheet = workbook.Worksheets.Add($"{curDep.Name}_{curType.Name}_{MonthsNames[grMonth.Key.Month - 1]}_{grMonth.First().Year}");
                            int rowIndexStart = 11;
                            int rowIndex = rowIndexStart;
                            PrintHeadersForMonthReport(sheet, rowIndex++, firstColumnIndex);
                            sheet.Cell(7, 3).Value = $"{MonthsNames[grMonth.Key.Month - 1]}";
                            sheet.Cell(7, 4).Value = grMonth.First().Year;
                            sheet.Cell($"R{6}").Value = grMonth.First().Year;
                            sheet.Cell(5, 2).FormulaA1 = depsExcelDict[curDep];

                            foreach (var row in grMonth)
                            {
                                if (row == null)
                                {
                                    throw new Exception("row is null");
                                }

                                var user = row.LinkingPart.Assignments.First().User;
                                var userRefs = usersExceDict[user];
                                var contracts = row.LinkingPart.Assignments.OrderByDescending(c => c.AssignmentDate);
                                var displayContract = contracts.Last();
                                var valueContract = contracts.First();
                                sheet.Cell($"O4").FormulaA1 = positionsPeopleRefsDict["Проректор по учебной работе"];
                                sheet.Cell($"{ListNumberColumnLetter}{rowIndex}").Value = rowIndex - rowIndexStart;
                                sheet.Range($"{HomeAddressAndPassportColumnLetterStart}{rowIndex}:{HomeAddressAndPassportColumnLetterEnd}{rowIndex}").Merge().FirstCell().FormulaA1 = userRefs.passportExcelRef;
                                sheet.Range($"{ContractColumnLetterStart}{rowIndex}:{ContractColumnLetterEnd}{rowIndex}").Merge().FirstCell().Value = $"{displayContract.ContractIdentifier} c {displayContract.PeriodStart.ToShortDateString()} по {displayContract.PeriodEnd.ToShortDateString()}";
                                sheet.Range($"{SumToBePadColumnLetterStart}{rowIndex}:{SumToBePadColumnLetterEnd}{rowIndex}").Merge().FirstCell().FormulaA1 = $"{HourPriceColumnLetter}{rowIndex} * {HoursAmmColumnLetter}{rowIndex}";
                                sheet.Cell($"{AllowedHoursColumnLetter}{rowIndex}").Value = valueContract.TimeSum;
                                var nspRange = sheet.Range($"{NSPColumnLetterStart}{rowIndex}:{NSPColumnLetterEnd}{rowIndex}").Merge();
                                nspRange.FirstCell().FormulaA1 = userRefs.NSPExcelRef;
                                sheet.Cell($"{DegreeColumnLetter}{rowIndex}").FormulaA1 = userRefs.degreeOnDateDict[row.MontYearAsDate].degreesExcelRefs;
                                sheet.Cell($"{HourPriceColumnLetter}{rowIndex}").FormulaA1 = $"{userRefs.degreeOnDateDict[row.MontYearAsDate].ratesExcelRefs} + {ctypesDict[curType].values[grMonth.Key]}";
                                sheet.Cell(rowIndex, firstColumnIndex).Value = row.LectionsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 1).Value = row.PracticalClassesTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 2).Value = row.LaboratoryClassesTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 3).Value = row.ConsultationsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 4).Value = row.OtherTeachingClassesTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 5).Value = row.CreditsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 6).Value = row.ExamsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 7).Value = row.CourseProjectsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 8).Value = row.InterviewsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 9).Value = row.TestsAndReferatsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 10).Value = row.InternshipsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 11).Value = row.DiplomasTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 12).Value = row.DiplomasReviewsTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 13).Value = row.SECTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 14).Value = row.GraduatesManagementTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 15).Value = row.GraduatesAcademicWorkTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 16).Value = row.PlasticPosesDemonstrationTime;
                                sheet.Cell(rowIndex, firstColumnIndex + 17).Value = row.TestingEscortTime;
                                var sumCell = sheet.Cell(rowIndex, firstColumnIndex + 18);
                                sheet.Cell(rowIndex, firstColumnIndex + 19).FormulaA1 = nspRange.FirstCell().Address.ToString();
                                sumCell.FormulaA1 = $"=Sum({sheet.Column(firstColumnIndex).ColumnLetter()}{rowIndex}:{sheet.Column(firstColumnIndex + 17).ColumnLetter()}{rowIndex})";
                                sheet.Cell($"O{rowIndex}").FormulaA1 = sumCell.Address.ToString();
                                var lValues = linkimgPartDict[row.LinkingPartID];
                                var prevDoneHoursCell = sheet.Cell($"{PrevDoneHoursColumnLetter}{rowIndex}");
                                if (lValues.IsRef)
                                {
                                    prevDoneHoursCell.FormulaA1 = lValues.formula;
                                }
                                else
                                {
                                    prevDoneHoursCell.Value = lValues.ReportsTimeSumValue;
                                }
                                sheet.Cell($"{LeftoverColumnLetter}{rowIndex}").FormulaA1 = $"{AllowedHoursColumnLetter}{rowIndex} - ({sumCell.Address} + {prevDoneHoursCell}{rowIndex})";

                                linkimgPartDict.Remove(row.LinkingPartID);
                                linkimgPartDict.Add(row.LinkingPartID, (row.LinkingPartID, -1, $"{prevDoneHoursCell.Worksheet.Name}!{prevDoneHoursCell.Address} + {prevDoneHoursCell.Worksheet.Name}!{sumCell.Address}", true ));

                                rowIndex++;
                            }

                            var leftTableRange = sheet.Range($"A{rowIndexStart}:S{rowIndex - 1}");

                            leftTableRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                            leftTableRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                            leftTableRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                            leftTableRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

                            var rightTableRange = sheet.Range($"U{rowIndexStart}:AN{rowIndex - 1}");

                            rightTableRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                            rightTableRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                            rightTableRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                            rightTableRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

                            rowIndexStart++;

                            var summartRange = sheet.Range($"{ContractColumnLetterStart}{rowIndex}:{HomeAddressAndPassportColumnLetterEnd}{rowIndex}");
                            summartRange.Merge().FirstCell().Value = $"Итого:";
                            summartRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            sheet.Cell($"{AllowedHoursColumnLetter}{rowIndex}").FormulaA1 = $"Sum({AllowedHoursColumnLetter}{rowIndexStart}:{AllowedHoursColumnLetter}{rowIndex - 1})";
                            sheet.Cell($"{PrevDoneHoursColumnLetter}{rowIndex}").FormulaA1 = $"Sum({PrevDoneHoursColumnLetter}{rowIndexStart}:{PrevDoneHoursColumnLetter}{rowIndex - 1})";
                            sheet.Cell($"{LeftoverColumnLetter}{rowIndex}").FormulaA1 = $"Sum({LeftoverColumnLetter}{rowIndexStart}:{LeftoverColumnLetter}{rowIndex - 1})";
                            sheet.Cell($"{HoursAmmColumnLetter}{rowIndex}").FormulaA1 = $"Sum({HoursAmmColumnLetter}{rowIndexStart}:{HoursAmmColumnLetter}{rowIndex - 1})";
                            sheet.Range($"{SumToBePadColumnLetterStart}{rowIndex}:{SumToBePadColumnLetterEnd}{rowIndex}").Merge().FirstCell().FormulaA1 = $"Sum({SumToBePadColumnLetterStart}{rowIndexStart}:{SumToBePadColumnLetterStart}{rowIndex - 1})";


                            sheet.Range($"B{rowIndex + 1}:F{rowIndex + 1}").Merge().Value = "Прошу оплатить за счет внебюджетных средств";

                            var moneyWordPlace = sheet.Range($"H{rowIndex + 1}:S{rowIndex + 1}").Merge().Style.Fill.BackgroundColor =XLColor.LightYellow;


                            for (int i = 0; i < Headers.Length; i++)
                            {
                                var column = sheet.Column(firstColumnIndex + i);
                                sheet.Cell(rowIndex, firstColumnIndex + i).FormulaA1 = $"Sum({column.ColumnLetter()}{rowIndexStart}:{column.ColumnLetter()}{rowIndex - 1})";
                            }
                            rowIndex+=3;
                            for (int i = 0; i < Headers.Length; i++)
                            {
                                var cellToPrint = sheet.Cell(rowIndex, 1 + i);
                                cellToPrint.Value = Headers[i];
                                cellToPrint.Style.Alignment.WrapText = true;
                            }

                            var bottomTableRange = sheet.Range($"A{rowIndex}:S{rowIndex + 1}");
                            bottomTableRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                            bottomTableRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                            bottomTableRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                            bottomTableRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
                            for (int i = 0; i < Headers.Length; i++)
                            {
                                sheet.Cell(rowIndex + 1, 1 + i).FormulaA1 = sheet.Cell(rowIndex - 4, firstColumnIndex + i).Address.ToString();
                            }


                            sheet.Range($"B{rowIndex + 3}:D{rowIndex + 3}").Merge().Value = "Работу принял:";


                            sheet.Range($"B{rowIndex + 5}:D{rowIndex + 5}").Merge().Value = "Зам.начальника ПЭУ";
                            sheet.Range($"E{rowIndex + 5}:F{rowIndex + 5}").Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            sheet.Range($"G{rowIndex + 5}:H{rowIndex + 5}").Merge().FirstCell().FormulaA1 = positionsPeopleRefsDict["Зам.начальника ПЭУ"];

                            sheet.Range($"B{rowIndex + 6}:D{rowIndex + 6}").Merge().Value = "Начальник УМУ";
                            sheet.Range($"E{rowIndex + 6}:F{rowIndex + 6}").Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            sheet.Range($"G{rowIndex + 6}:H{rowIndex + 6}").Merge().FirstCell().FormulaA1 = positionsPeopleRefsDict["Начальник УМУ"];

                            sheet.Range($"B{rowIndex + 7}:D{rowIndex + 7}").Merge().Value = "Декан факультета";
                            sheet.Range($"E{rowIndex + 7}:F{rowIndex + 7}").Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            sheet.Range($"G{rowIndex + 7}:H{rowIndex + 7}").Merge().FirstCell().FormulaA1 = positionsPeopleRefsDict["Декан факультета"];

                            sheet.Range($"B{rowIndex + 8}:D{rowIndex + 8}").Merge().Value = "Заведующий кафедрой";
                            sheet.Range($"E{rowIndex + 8}:F{rowIndex + 8}").Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            sheet.Range($"G{rowIndex + 8}:H{rowIndex + 8}").Merge().FirstCell().FormulaA1 = positionsPeopleRefsDict["Заведующий кафедрой"]; ;
                        }
                    }
                }

                workbook.SaveAs(path);
            }

            return path;
        } 

        private async Task<Dictionary<ContractType, (string nameRef, Dictionary<DateTime, string> values)>> AddContractTypesWithAssignments(IXLWorksheet sheet, IEnumerable<ContractType> degrees, IEnumerable<DateTime> dates, CancellationToken token = default) 
        {
            sheet.Cell(1, 1).Value = "Наименование";
            int rowInddexToPrintValue = 2;
            int columnInddexToPrintMonthYear = 2;
            int counter = 0;
            Dictionary<DateTime, string> dateColumnDict = new Dictionary<DateTime, string>();
            foreach (var date in dates)
            {
                var theCell = sheet.Cell(1, columnInddexToPrintMonthYear + counter);
                theCell.Value = $"{MonthsNames[date.Month - 1]} {date.Year}";
                dateColumnDict.Add(date, theCell.WorksheetColumn().ColumnLetter());
                counter++;
            }

            Dictionary<ContractType, (string nameRef, Dictionary<DateTime, string> values)> dict = new();
            foreach (var degree in Enumerable.Distinct<ContractType>(degrees))
            {
                var nameCell = sheet.Cell(rowInddexToPrintValue, 1);
                nameCell.Value = degree.Name;
                string nameCellRef = $"{sheet.Name}!{nameCell.Address}";
                var dateValues = await _contractTypeService.GetAssignmentForEachMonthOnPeriodAsync(degree.ID, dates.Min(), dates.Max(), token);
                Dictionary<DateTime, string> dateValueList = new();
                foreach (var dateValue in dateValues)
                {
                    var theCell = sheet.Cell($"{dateColumnDict[dateValue.date]}{rowInddexToPrintValue}");
                    theCell.Value = dateValue.value == null ? 0 : dateValue.value.Value;
                    dateValueList.Add(dateValue.date, $"{sheet.Name}!{theCell.Address}");
                }

                dict.Add(degree, (nameCellRef, dateValueList));
                rowInddexToPrintValue++;
            }

            return dict;
        }

        private async Task<Dictionary<AcademicDegree, (string nameRef, Dictionary<DateTime, string> values)>> AddAcademicDegreesWithAssignments(IXLWorksheet sheet, IEnumerable<AcademicDegree> degrees, IEnumerable<DateTime> dates, CancellationToken token = default) 
        {
            sheet.Cell(1, 1).Value = "Наименование";
            int rowInddexToPrintDegree = 2;
            int columnInddexToPrintMonthYear = 2;
            int counter = 0;
            Dictionary<DateTime, string> dateColumnDict = new Dictionary<DateTime, string>();
            foreach (var date in dates)
            {
                var theCell = sheet.Cell(1, columnInddexToPrintMonthYear + counter);
                theCell.Value = $"{MonthsNames[date.Month - 1]} {date.Year}";
                dateColumnDict.Add(date, theCell.WorksheetColumn().ColumnLetter());
                counter++;
            }

            var updatedDegrees = degrees.ToList();
            updatedDegrees.Add(DefaultDegree);
            Dictionary<AcademicDegree, (string nameRef, Dictionary<DateTime, string> values)> dict = new();
            foreach (var degree in updatedDegrees.Distinct())
            {
                var nameCell = sheet.Cell(rowInddexToPrintDegree, 1);
                nameCell.Value = degree.Name;
                string nameCellRef = $"{sheet.Name}!{nameCell.Address}";
                var dateValues = await _academicDegreeService.GetAssignmentForEachMonthOnPeriodAsync(degree.ID, dates.Min(), dates.Max(), token);
                Dictionary<DateTime, string> dateValueList = new();
                foreach (var dateValue in dateValues)
                {
                    var theCell = sheet.Cell($"{dateColumnDict[dateValue.date]}{rowInddexToPrintDegree}");
                    theCell.Value = dateValue.value == null ? 0 : dateValue.value.Value;
                    dateValueList.Add(dateValue.date, $"{sheet.Name}!{theCell.Address}");
                }

                dict.Add(degree, (nameCellRef, dateValueList));
                rowInddexToPrintDegree++;
            }

            return dict;
        }


        private async Task<Dictionary<User, (string passportExcelRef, string NSPExcelRef, Dictionary<DateTime, (string degreesExcelRefs, string ratesExcelRefs)> degreeOnDateDict)>> AddUsersOnPage(IXLWorksheet sheet, IEnumerable<User> users, Dictionary<AcademicDegree, (string nameRef, Dictionary<DateTime, string> values)> degreesDict, IEnumerable<DateTime> dates)
        {
            sheet.Cell(1, 1).Value = "ФИО";
            sheet.Cell(1, 2).Value = "Паспортные данные";
            Dictionary<User, (string passportExcelRef, string NSPExcelRef, Dictionary<DateTime, (string degreesExcelRefs, string ratesExcelRefs)> degreeOnDateDict)> usersExceDict = new();
            int userPrintRowIndex = 2;
            var degreesKeys = degreesDict.Keys.ToList();

            foreach (var user in users.Distinct())
            {
                var NSPCell = sheet.Cell(userPrintRowIndex, 1);
                NSPCell.Value = user.NSP;
                var passportCell = sheet.Cell(userPrintRowIndex, 2);
                passportCell.Value = user.NSP + " PassportData";
                string passportCellRef = $"{sheet.Name}!{passportCell.Address}";
                string NSPCellRef = $"{sheet.Name}!{NSPCell.Address}";

                sheet.Cell(userPrintRowIndex, 3).Value = "Степень";
                sheet.Cell(userPrintRowIndex + 1, 3).Value = "Ставка";
                var dateValues = await _accountService.GetAssignmentForEachMonthOnPeriodAsync(user.ID, dates.Min(), dates.Max());
                var counter = 0;
                Dictionary<DateTime, (string degreesExcelRefs, string ratesExcelRefs)> degreeOnDateDict = new();
                foreach (var dateValue in dateValues)
                {
                    var degreeCell = sheet.Cell(userPrintRowIndex, 3 + counter);
                    var rateCell = sheet.Cell(userPrintRowIndex + 1, 3 + counter);
                    AcademicDegree key;
                    if (dateValue.value == null)
                    {
                        key = DefaultDegree;
                    }
                    else
                    {
                        key = degreesKeys.First(d => d.ID == dateValue.value.Value);
                    }

                    var degreeRefs = degreesDict[key];

                    degreeCell.FormulaA1 = degreeRefs.nameRef;
                    rateCell.FormulaA1 = degreeRefs.values[dateValue.date];

                    degreeOnDateDict.Add(dateValue.date, ($"{sheet.Name}!{degreeCell.Address}", $"{sheet.Name}!{rateCell.Address}"));
                    counter++;
                }


                usersExceDict.Add(user, (passportCellRef, NSPCellRef, degreeOnDateDict));
                userPrintRowIndex += 2;
            }

            return usersExceDict;
        }

        private Dictionary<Department, string> AddDepartmentsOnPage(IXLWorksheet userSheet, IEnumerable<Department> deps)
        {
            userSheet.Cell(1, 1).Value = "Имя отдела";
            Dictionary<Department, string> depsExcelrefsDict = new();
            int firstPrintRowIndex = 2;
            foreach (var dep in deps.Distinct())
            {
                var nameCell = userSheet.Cell(firstPrintRowIndex, 1);
                nameCell.Value = dep.Name;
                string NSPCellRef = $"{userSheet.Name}!{nameCell.Address}";
                depsExcelrefsDict.Add(dep, NSPCellRef);
                firstPrintRowIndex++;
            }

            return depsExcelrefsDict;
        }

        private void PrintHeadersForMonthReport(IXLWorksheet sheet, int reportsTablePartHeadersRowIndex, int reportsTablePartHeadersFirstColumnIndex)
        {

            sheet.Range($"{ListNumberColumnLetter}{headerStart}:{ListNumberColumnLetter}{headerEnd}").Merge().Value = "N п/п";
            sheet.Range($"{NSPColumnLetterStart}{headerStart}:{NSPColumnLetterEnd}{headerEnd}").Merge().Value = "Фамилия, имя, отчество";
            sheet.Range($"{DegreeColumnLetter}{headerStart}:{DegreeColumnLetter}{headerEnd}").Merge().Value = "Ученое звание, степень";
            sheet.Range($"{ContractColumnLetterStart}{headerStart}:{ContractColumnLetterEnd}{headerEnd}").Merge().Value = "Номер, дата, срок действия договора";
            sheet.Range($"{HomeAddressAndPassportColumnLetterStart}{headerStart}:{HomeAddressAndPassportColumnLetterEnd}{headerEnd}").Merge().Value = "Домашний адрес с индексом. Паспорт:серия,номер,выданный.\r\nМесто основной работы";
            sheet.Range($"{AllowedHoursColumnLetter}{headerEnd - 1}:{AllowedHoursColumnLetter}{headerEnd}").Merge().Value = "Разрешено часов по договору";
            sheet.Range($"{PrevDoneHoursColumnLetter}{headerEnd - 1}:{PrevDoneHoursColumnLetter}{headerEnd}").Merge().Value = "Выполнено за предыдущие месяцы";
            sheet.Range($"{HourlyPaidWorkHeaderColumnLetterStart}{headerStart}:{HourlyPaidWorkHeaderColumnLetterEnd}{headerStart}").Merge().Value = "Работа на условиях почасовой оплаты";
            sheet.Range($"{ToBeCalcucatedByAccountingColumnLetterStart}{headerEnd - 1}:{ToBeCalcucatedByAccountingColumnLetterEnd}{headerEnd - 1}").Merge().Value = "Подлежит расчету в бухгалтерии";
            sheet.Range($"{LeftoverColumnLetter}{headerEnd}").Value = "Остаток";
            sheet.Range($"{HoursAmmColumnLetter}{headerEnd}").Value = "Кол-во часов";
            sheet.Range($"{HourPriceColumnLetter}{headerEnd}").Value = "Расценка за 1 час";
            sheet.Range($"{SumToBePadColumnLetterStart}{headerEnd}:{SumToBePadColumnLetterEnd}{headerEnd}").Merge().Value = "Сумма к расчету";
            sheet.Range($"{ProffesorSignColumnLetter}{headerEnd}").Value = "Роспись преподавателя";


            sheet.Columns($"{ListNumberColumnLetter}:{ProffesorSignColumnLetter}").AdjustToContents();
            var leftHeadersRange = sheet.Range($"{ListNumberColumnLetter}{headerStart}:{ProffesorSignColumnLetter}{headerEnd}");
            leftHeadersRange.Style.Alignment.WrapText = true;
            leftHeadersRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            leftHeadersRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            leftHeadersRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            leftHeadersRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            leftHeadersRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            leftHeadersRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);


            sheet.Cell(ZaPageAddres).Value = "За";


            var headersRange = sheet.Range(reportsTablePartHeadersRowIndex - 2, reportsTablePartHeadersFirstColumnIndex, reportsTablePartHeadersRowIndex, reportsTablePartHeadersFirstColumnIndex + Headers.Length);
            headersRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            headersRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headersRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            headersRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            headersRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            headersRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            headersRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            headersRange.Style.Alignment.SetTextRotation(90);
            headersRange.Style.Alignment.WrapText = true;
            sheet.Rows($"{headersRange.Rows().First().RowNumber()}:{headersRange.Rows().Last().RowNumber()}").AdjustToContents();
            for (int i = 0; i < Headers.Length; i++)
            {
                sheet.Range(reportsTablePartHeadersRowIndex - 2, reportsTablePartHeadersFirstColumnIndex + i, reportsTablePartHeadersRowIndex, reportsTablePartHeadersFirstColumnIndex + i).Merge().Value = Headers[i];
            }
            sheet.Range(reportsTablePartHeadersRowIndex - 2, reportsTablePartHeadersFirstColumnIndex + Headers.Length, reportsTablePartHeadersRowIndex, reportsTablePartHeadersFirstColumnIndex + Headers.Length).Merge().Value = "Фамилия, имя, отчество";

            sheet.Cell(2, 2).Value = "АКТ ПРИЕМКИ";
            sheet.Cell(3, 2).Value = "работ, выполненных штатными преподавателями";
            sheet.Cell(4, 2).Value = "на условиях почасовой оплаты на кафедре";
            sheet.Cell("O2").Value = "УТВЕРЖДАЮ";
            sheet.Cell("O3").Value = "Проректор по учебной работе";
            sheet.Cell("O6").Value = "\"______\"";
            sheet.Range("O5:P5").Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            sheet.Range("P6:Q6").Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
        }
    }
}
