using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Logic.Services
{
    public class ContractService : IContractService
    {
        private readonly IAppDBContext _appDBContext;
        private readonly IAccountService _accountService;
        public ContractService(IAppDBContext appDBContext, IAccountService accountService)
        {
            _appDBContext = appDBContext;
            _accountService = accountService;
        }

        public async Task<IEnumerable<Contract>> GetAll()
        {
            return await _appDBContext.Contracts.ToListAsync();
        }

        public async Task<Contract?> GetContractAsync(int id) => 
            await _appDBContext.Contracts.FirstOrDefaultAsync(c => c.ID == id);

        public async Task Add(Contract contract)
        {
            if (contract.ParentContractID != null)
            {
                contract.ParentContract = await GetContractAsync(contract.ParentContractID.Value) ?? throw new ObjectNotFoundException($"Contract with id = {contract.ParentContractID} not found");
                contract.PeriodEnd = contract.ParentContract.PeriodEnd;
                contract.PeriodStart = contract.ParentContract.PeriodStart;
                contract.DepartmentID = contract.ParentContract.DepartmentID;
            }

            var res = ValidateOnCreate(contract);
            if (res.Any())
            {
                throw new ArgumentException(string.Join("\n", res));
            }

            _appDBContext.Contracts.Add(contract);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task Edit(Contract contract)
        {
            var sourceContract = await GetContractAsync(contract.ID) ?? throw new ObjectNotFoundException($"Contract wasn't found with ID = {contract.ID}");
            if (sourceContract.IsConfirmed)
            {
                throw new ArgumentException($"Contract(ID = {contract.ID}) is confirmed. No edition allowed");
            }

            var res = ValidateOnCreate(contract);
            if (res.Any())
            {
                throw new ArgumentException(string.Join("\n", res));
            }

            await Delete(sourceContract);
            await Add(sourceContract);
        }

        public async Task Delete(int id)
        {
            var contract = await GetContractAsync(id);
            if (contract != null)
            {
                await Delete(contract);
            }
        }

        public async Task Delete(Contract contract)
        {
            if (contract == null)
            {
                return;
            }

            var childContract = await _appDBContext.Contracts.FirstOrDefaultAsync(c => c.ParentContractID  == contract.ID);
            if (childContract != null)
            {
                childContract.ParentContractID = null;
            }

            _appDBContext.Contracts.Remove(contract);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task ConfirmContractAsync(int contractID, string adminLogin)
        {
            var user = await _accountService.GetUserAsync(adminLogin) ?? throw new ObjectNotFoundException($"User with login = '{adminLogin}' not found");
            if (!_accountService.IsAdmin(user))
            {
                throw new NoAccessException();
            }

            var contract = await GetContractAsync(contractID) ?? throw new ObjectNotFoundException($"Contract wasn't found by ID = {contractID}");
            if (contract.IsConfirmed)
            {
                throw new ArgumentException("This contract is already confirmed");
            }

            contract.ConfirmedByUserID = user.ID;
            contract.MonthReports = GenerateMonthReportsForContract(contract);
            await _appDBContext.SaveChangesAsync();
        }
      
        public async Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID)
        {
            var contract = await GetContractAsync(contractID) 
                ?? throw new KeyNotFoundException($"Contract wasn't found by ID = {contractID}"); 
            
            if (!contract.IsConfirmed)
            {
                throw new ArgumentException($"Contract(ID = {contract.ID}) is not confirmed");
            }

            return _appDBContext.GetMonthReportsRecursively(contractID) ?? throw new Exception();
        }
        
        public async Task UpdateMonthReport(MonthReport monthReportToAply)
        {
            var report = await _appDBContext.MonthReports.FirstOrDefaultAsync(r => r.ContractID == monthReportToAply.ContractID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year) 
                ?? throw new ObjectNotFoundException($"Month report not found by key[ID = {monthReportToAply.ContractID}, Month = {monthReportToAply.Month}, Year = {monthReportToAply.Year}]");
            var untakenTimeIfRemoveTheReport = await GetMonthReportsUntakenTime(report.ContractID, new[] { (report.ContractID, report.Year, report.Month) });

            if (untakenTimeIfRemoveTheReport.TimeSum < monthReportToAply.TimeSum) 
                throw new ArgumentException("untakenTimeIfRemoveTheReport.TimeSum > monthReportToAply.TimeSum");
            if (untakenTimeIfRemoveTheReport.LectionsTime < monthReportToAply.LectionsTime) 
                throw new ArgumentException("untakenTimeIfRemoveTheReport.LectionsTime > monthReportToAply.LectionsTime");
            if (untakenTimeIfRemoveTheReport.PracticalClassesTime < monthReportToAply.PracticalClassesTime) 
                throw new ArgumentException("untakenTimeIfRemoveTheReport.PracticalClassesTime < monthReportToAply.PracticalClassesTime");
            if (untakenTimeIfRemoveTheReport.LaboratoryClassesTime < monthReportToAply.LaboratoryClassesTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.LaboratoryClassesTime > monthReportToAply.LaboratoryClassesTime");
            if (untakenTimeIfRemoveTheReport.ConsultationsTime < monthReportToAply.ConsultationsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.ConsultationsTime > monthReportToAply.ConsultationsTime");
            if (untakenTimeIfRemoveTheReport.OtherTeachingClassesTime < monthReportToAply.OtherTeachingClassesTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.OtherTeachingClassesTime > monthReportToAply.OtherTeachingClassesTime");
            if (untakenTimeIfRemoveTheReport.CreditsTime < monthReportToAply.CreditsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.CreditsTime > monthReportToAply.CreditsTime");
            if (untakenTimeIfRemoveTheReport.ExamsTime < monthReportToAply.ExamsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.ExamsTime > monthReportToAply.ExamsTime");
            if (untakenTimeIfRemoveTheReport.CourseProjectsTime < monthReportToAply.CourseProjectsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.CourseProjectsTime > monthReportToAply.CourseProjectsTime");
            if (untakenTimeIfRemoveTheReport.InterviewsTime < monthReportToAply.InterviewsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.InterviewsTime > monthReportToAply.InterviewsTime");
            if (untakenTimeIfRemoveTheReport.TestsAndReferatsTime < monthReportToAply.TestsAndReferatsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.TestsAndReferatsTime > monthReportToAply.TestsAndReferatsTime");
            if (untakenTimeIfRemoveTheReport.InternshipsTime < monthReportToAply.InternshipsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.InternshipsTime > monthReportToAply.InternshipsTime");
            if (untakenTimeIfRemoveTheReport.DiplomasTime < monthReportToAply.DiplomasTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.DiplomasTime > monthReportToAply.DiplomasTime");
            if (untakenTimeIfRemoveTheReport.DiplomasReviewsTime < monthReportToAply.DiplomasReviewsTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.DiplomasReviewsTime > monthReportToAply.DiplomasReviewsTime");
            if (untakenTimeIfRemoveTheReport.SECTime < monthReportToAply.SECTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.SECTime > monthReportToAply.SECTime");
            if (untakenTimeIfRemoveTheReport.GraduatesManagementTime < monthReportToAply.GraduatesManagementTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.GraduatesManagementTime > monthReportToAply.GraduatesManagementTime");
            if (untakenTimeIfRemoveTheReport.GraduatesAcademicWorkTime < monthReportToAply.GraduatesAcademicWorkTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.GraduatesAcademicWorkTime > monthReportToAply.GraduatesAcademicWorkTime");
            if (untakenTimeIfRemoveTheReport.PlasticPosesDemonstrationTime < monthReportToAply.PlasticPosesDemonstrationTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.PlasticPosesDemonstrationTime > monthReportToAply.PlasticPosesDemonstrationTime");
            if (untakenTimeIfRemoveTheReport.TestingEscortTime < monthReportToAply.TestingEscortTime)
                throw new ArgumentException("untakenTimeIfRemoveTheReport.TestingEscortTime > monthReportToAply.TestingEscortTime"); 

            _appDBContext.MonthReports.Remove(report);
            _appDBContext.MonthReports.Add(monthReportToAply);
            await _appDBContext.SaveChangesAsync();
        }

        private static IEnumerable<MonthReport> GenerateMonthReportsForContract(Contract contract)
        {
            var dateStart = new DateTime(contract.PeriodStart.Year, contract.PeriodStart.Month, 1);
            var dateEnd = new DateTime(contract.PeriodEnd.Year, contract.PeriodEnd.Month, 1);
            var reports = new List<MonthReport>();
            while (dateStart <= dateEnd)
            {
                reports.Add(new MonthReport { Contract = contract, ContractID = contract.ID, Month = dateStart.Month, Year = dateStart.Year });
                dateStart = dateStart.AddMonths(1);
            }

            return reports;
        }

        public async Task<IEnumerable<Contract>> GetUserContracts(string UserName) => await _appDBContext.Contracts.Where(c => c.User.Name == UserName).ToListAsync();

        public static IEnumerable<ValidationResult> ValidateOnCreate(Contract contract)
        {
            if (contract.ParentContractID != null)
            {
                if (contract.ParentContract == null)
                {
                    throw new Exception($"{nameof(contract.ParentContract)} is null");
                }

                if (contract.ParentContractID != contract.ParentContract.ID)
                {
                    throw new Exception("contract.ParentContractID != contract.ParentContract.ID");
                }

                if (contract.UserID != contract.ParentContract.UserID)
                {
                    yield return new ValidationResult($"UserID != ParentContract.UserID");
                }

                if (!contract.ParentContract.IsConfirmed)
                {
                    yield return new ValidationResult($"Parent contract is not confirmed");
                }

                if (contract.TimeSum <= contract.ParentContract.TimeSum)
                {
                    yield return new ValidationResult($"TimeSum <= ParentContract.TimeSum");
                }

                if (contract.LectionsMaxTime < contract.ParentContract.LectionsMaxTime)
                {
                    yield return new ValidationResult($"LectionsMaxTime < ParentContract.LectionsMaxTime");
                }

                if (contract.PracticalClassesMaxTime < contract.ParentContract.PracticalClassesMaxTime)
                {
                    yield return new ValidationResult($"PracticalClassesMaxTime < ParentContract.PracticalClassesMaxTime");
                }

                if (contract.LaboratoryClassesMaxTime < contract.ParentContract.LaboratoryClassesMaxTime)
                {
                    yield return new ValidationResult($"LaboratoryClassesMaxTime < ParentContract.LaboratoryClassesMaxTime");
                }

                if (contract.ConsultationsMaxTime < contract.ParentContract.ConsultationsMaxTime)
                {
                    yield return new ValidationResult($"ConsultationsMaxTime < ParentContract.ConsultationsMaxTime");
                }

                if (contract.OtherTeachingClassesMaxTime < contract.ParentContract.OtherTeachingClassesMaxTime)
                {
                    yield return new ValidationResult($"OtherTeachingClassesMaxTime < ParentContract.OtherTeachingClassesMaxTime");
                }

                if (contract.CreditsMaxTime < contract.ParentContract.CreditsMaxTime)
                {
                    yield return new ValidationResult($"CreditsMaxTime < ParentContract.CreditsMaxTime");
                }

                if (contract.ExamsMaxTime < contract.ParentContract.ExamsMaxTime)
                {
                    yield return new ValidationResult($"ExamsMaxTime < ParentContract.ExamsMaxTime");
                }

                if (contract.CourseProjectsMaxTime < contract.ParentContract.CourseProjectsMaxTime)
                {
                    yield return new ValidationResult($"CourseProjectsMaxTime < ParentContract.CourseProjectsMaxTime");
                }

                if (contract.InterviewsMaxTime < contract.ParentContract.InterviewsMaxTime)
                {
                    yield return new ValidationResult($"InterviewsMaxTime < ParentContract.InterviewsMaxTime");
                }

                if (contract.TestsAndReferatsMaxTime < contract.ParentContract.TestsAndReferatsMaxTime)
                {
                    yield return new ValidationResult($"TestsAndReferatsMaxTime < ParentContract.TestsAndReferatsMaxTime");
                }

                if (contract.InternshipsMaxTime < contract.ParentContract.InternshipsMaxTime)
                {
                    yield return new ValidationResult($"InternshipsMaxTime < ParentContract.InternshipsMaxTime");
                }

                if (contract.DiplomasMaxTime < contract.ParentContract.DiplomasMaxTime)
                {
                    yield return new ValidationResult($"DiplomasMaxTime < ParentContract.DiplomasMaxTime");
                }

                if (contract.DiplomasReviewsMaxTime < contract.ParentContract.DiplomasReviewsMaxTime)
                {
                    yield return new ValidationResult($"DiplomasReviewsMaxTime < ParentContract.DiplomasReviewsMaxTime");
                }

                if (contract.SECMaxTime < contract.ParentContract.SECMaxTime)
                {
                    yield return new ValidationResult($"SECMaxTime < ParentContract.SECMaxTime");
                }

                if (contract.GraduatesManagementMaxTime < contract.ParentContract.GraduatesManagementMaxTime)
                {
                    yield return new ValidationResult($"GraduatesManagementMaxTime < ParentContract.GraduatesManagementMaxTime");
                }

                if (contract.GraduatesAcademicWorkMaxTime < contract.ParentContract.GraduatesAcademicWorkMaxTime)
                {
                    yield return new ValidationResult($"GraduatesAcademicWorkMaxTime < ParentContract.GraduatesAcademicWorkMaxTime");
                }

                if (contract.PlasticPosesDemonstrationMaxTime < contract.ParentContract.PlasticPosesDemonstrationMaxTime)
                {
                    yield return new ValidationResult($"PlasticPosesDemonstrationMaxTime < ParentContract.PlasticPosesDemonstrationMaxTime");
                }

                if (contract.TestingEscortMaxTime < contract.ParentContract.TestingEscortMaxTime)
                {
                    yield return new ValidationResult($"TestingEscortMaxTime < ParentContract.TestingEscortMaxTime");
                }
            }
            else
            {
                if (contract == null)
                {
                    throw new ArgumentNullException(nameof(contract));
                }

                if (contract.ParentContractID != null && contract.ParentContract == null)
                {
                    throw new ArgumentNullException(nameof(contract), $"{contract.ParentContract} is null");
                }

                if (contract.PeriodStart > contract.PeriodEnd)
                {
                    yield return new ValidationResult($"{nameof(contract.PeriodStart)} > {nameof(contract.PeriodEnd)}");
                }

                if (contract.TimeSum < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.TimeSum)} < 0");
                }

                if (contract.LectionsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.LectionsMaxTime)} < 0");
                }

                if (contract.PracticalClassesMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.PracticalClassesMaxTime)} < 0");
                }

                if (contract.LaboratoryClassesMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.LaboratoryClassesMaxTime)} < 0");
                }

                if (contract.ConsultationsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.ConsultationsMaxTime)} < 0");
                }

                if (contract.OtherTeachingClassesMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.OtherTeachingClassesMaxTime)} < 0");
                }

                if (contract.CreditsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.CreditsMaxTime)} < 0");
                }

                if (contract.ExamsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.ExamsMaxTime)} < 0");
                }

                if (contract.CourseProjectsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.CourseProjectsMaxTime)} < 0");
                }

                if (contract.InterviewsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.InterviewsMaxTime)} < 0");
                }

                if (contract.TestsAndReferatsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.TestsAndReferatsMaxTime)} < 0");
                }

                if (contract.InternshipsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.InternshipsMaxTime)} < 0");
                }

                if (contract.DiplomasMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.DiplomasMaxTime)} < 0");
                }

                if (contract.DiplomasReviewsMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.DiplomasReviewsMaxTime)} < 0");
                }

                if (contract.SECMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.SECMaxTime)} < 0");
                }

                if (contract.GraduatesManagementMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.GraduatesManagementMaxTime)} < 0");
                }

                if (contract.GraduatesAcademicWorkMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.GraduatesAcademicWorkMaxTime)} < 0");
                }

                if (contract.PlasticPosesDemonstrationMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.PlasticPosesDemonstrationMaxTime)} < 0");
                }

                if (contract.TestingEscortMaxTime < 0)
                {
                    yield return new ValidationResult($"{nameof(contract.TestingEscortMaxTime)} < 0");
                }
            }
        }

        public async Task<List<MonthReport>> GetMonthReportAsyncOnDate(DateTime date)
        {
            return await _appDBContext.MonthReports.Include(m => m.Contract).ThenInclude(c => c.User).Where(m => m.Year == date.Year && m.Month == date.Month).ToListAsync();
        }

        public async Task<string?> GetOwnersLoginAsync(int contractID)
        {
            return (await _appDBContext.Contracts.Include(c => c.User).FirstOrDefaultAsync(c => c.ID == contractID))?.User.Login ?? null;
        }

        public async Task<MonthReportsUntakenTimeModel> GetMonthReportsUntakenTime(int contractID, IEnumerable<(int contractID, int year, int month)> exceptValuesWithKeys)
        {
            var clearedReports = (await GetMonthReportsAsync(contractID))
                .Where(r => !exceptValuesWithKeys.Any(e => e.contractID == r.ContractID && e.year == r.Year && e.month == r.Year));
            var contact = await GetContractAsync(contractID)
                ?? throw new KeyNotFoundException($"Contract wasn't found by ID = {contractID}");

            return new MonthReportsUntakenTimeModel
            {
                ContractID = contractID,
                ExceptForReportsWithKey = exceptValuesWithKeys,
                LectionsTime = contact.LectionsMaxTime - clearedReports.Sum(c => c.LectionsTime),
                PracticalClassesTime = contact.PracticalClassesMaxTime - clearedReports.Sum(c => c.PracticalClassesTime),
                LaboratoryClassesTime = contact.LaboratoryClassesMaxTime - clearedReports.Sum(c => c.LaboratoryClassesTime),
                ConsultationsTime = contact.ConsultationsMaxTime - clearedReports.Sum(c => c.ConsultationsTime),
                OtherTeachingClassesTime = contact.OtherTeachingClassesMaxTime - clearedReports.Sum(c => c.OtherTeachingClassesTime),
                CreditsTime = contact.CreditsMaxTime - clearedReports.Sum(c => c.CreditsTime),
                ExamsTime = contact.ExamsMaxTime - clearedReports.Sum(c => c.ExamsTime),
                CourseProjectsTime = contact.CourseProjectsMaxTime - clearedReports.Sum(c => c.CourseProjectsTime),
                InterviewsTime = contact.InterviewsMaxTime - clearedReports.Sum(c => c.InterviewsTime),
                TestsAndReferatsTime = contact.TestsAndReferatsMaxTime - clearedReports.Sum(c => c.TestsAndReferatsTime),
                InternshipsTime = contact.InternshipsMaxTime - clearedReports.Sum(c => c.InternshipsTime),
                DiplomasTime = contact.DiplomasMaxTime - clearedReports.Sum(c => c.DiplomasTime),
                DiplomasReviewsTime = contact.DiplomasReviewsMaxTime - clearedReports.Sum(c => c.DiplomasReviewsTime),
                SECTime = contact.SECMaxTime - clearedReports.Sum(c => c.SECTime),
                GraduatesManagementTime = contact.GraduatesManagementMaxTime - clearedReports.Sum(c => c.GraduatesManagementTime),
                GraduatesAcademicWorkTime = contact.GraduatesAcademicWorkMaxTime - clearedReports.Sum(c => c.GraduatesAcademicWorkTime),
                PlasticPosesDemonstrationTime = contact.PlasticPosesDemonstrationMaxTime - clearedReports.Sum(c => c.PlasticPosesDemonstrationTime),
                TestingEscortTime = contact.TestingEscortMaxTime - clearedReports.Sum(c => c.TestingEscortTime),
            };
        }
    }
}
