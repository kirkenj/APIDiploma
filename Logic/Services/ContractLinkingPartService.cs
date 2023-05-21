using Database.Entities;
using Database.Interfaces;
using Irony.Parsing;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.ContractLinkingPart;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class ContractLinkingPartService : IContractLinkingPartService
    {
        private readonly IMonthReportService _monthReportService;
        private readonly IAccountService _accountService;

        public DbSet<Contract> AssignmentsDBSet {get; set;}
        public DbSet<ContractLinkingPart> DbSet { get; set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; set; }
        
        public ContractLinkingPartService(IAppDBContext appDBContext, IMonthReportService monthReportService, IAccountService accountService)
        {
            AssignmentsDBSet = appDBContext.Set<Contract>();
            DbSet = appDBContext.Set<ContractLinkingPart>();
            _monthReportService = monthReportService;
            _accountService = accountService;
            SaveChangesAsync = appDBContext.SaveChangesAsync;
        }

        public async Task AddAsync(ContractLinkingPart entity, bool SaveChanges = true, CancellationToken token = default)
        {
            DbSet.Add(entity);
            if (SaveChanges)
            {
                await SaveChangesAsync(token);
            }
        }

        public Task UpdateAsync(ContractLinkingPart valueToAply, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Contract>> GetRelatedContractsAsync(int linkingPartID, CancellationToken token = default) => await AssignmentsDBSet.Where(c => c.LinkingPartID == linkingPartID).ToListAsync(token);

        public async Task UpdateMonthReport(MonthReport monthReportToAply)
        {
            var linkingPart = await DbSet.Include(l => l.MonthReports).Include(l => l.Assignments.Where(a => a.IsConfirmed)).FirstOrDefaultAsync(l =>l.MonthReports.Any(r => r.LinkingPartID == monthReportToAply.LinkingPartID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year))
                ?? throw new ObjectNotFoundException($"Month report not found by key[ID = {monthReportToAply.LinkingPartID}, Month = {monthReportToAply.Month}, Year = {monthReportToAply.Year}]");
            var report = linkingPart.MonthReports.First(r => r.LinkingPartID == monthReportToAply.LinkingPartID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year);

            if (report.IsBlocked)
            {
                throw new InvalidOperationException("This month report is blocked for edition");
            }

            foreach (var contract in linkingPart.Assignments.Where(a => a.IsConfirmed))
            {
                var untakenTimeIfRemoveTheReport = GetUntakenTime(report.LinkingPart, new DateTime(contract.AssignmentDate.Year, contract.AssignmentDate.Month, 1), new[] { (report.Year, report.Month) });
                if (untakenTimeIfRemoveTheReport.TimeSum < monthReportToAply.TimeSum)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.TimeSum > monthReportToAply.TimeSum where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.LectionsTime < monthReportToAply.LectionsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.LectionsTime > monthReportToAply.LectionsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.PracticalClassesTime < monthReportToAply.PracticalClassesTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.PracticalClassesTime < monthReportToAply.PracticalClassesTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.LaboratoryClassesTime < monthReportToAply.LaboratoryClassesTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.LaboratoryClassesTime > monthReportToAply.LaboratoryClassesTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.ConsultationsTime < monthReportToAply.ConsultationsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.ConsultationsTime > monthReportToAply.ConsultationsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.OtherTeachingClassesTime < monthReportToAply.OtherTeachingClassesTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.OtherTeachingClassesTime > monthReportToAply.OtherTeachingClassesTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.CreditsTime < monthReportToAply.CreditsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.CreditsTime > monthReportToAply.CreditsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.ExamsTime < monthReportToAply.ExamsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.ExamsTime > monthReportToAply.ExamsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.CourseProjectsTime < monthReportToAply.CourseProjectsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.CourseProjectsTime > monthReportToAply.CourseProjectsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.InterviewsTime < monthReportToAply.InterviewsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.InterviewsTime > monthReportToAply.InterviewsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.TestsAndReferatsTime < monthReportToAply.TestsAndReferatsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.TestsAndReferatsTime > monthReportToAply.TestsAndReferatsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.InternshipsTime < monthReportToAply.InternshipsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.InternshipsTime > monthReportToAply.InternshipsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.DiplomasTime < monthReportToAply.DiplomasTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.DiplomasTime > monthReportToAply.DiplomasTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.DiplomasReviewsTime < monthReportToAply.DiplomasReviewsTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.DiplomasReviewsTime > monthReportToAply.DiplomasReviewsTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.SECTime < monthReportToAply.SECTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.SECTime > monthReportToAply.SECTime");
                if (untakenTimeIfRemoveTheReport.GraduatesManagementTime < monthReportToAply.GraduatesManagementTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.GraduatesManagementTime > monthReportToAply.GraduatesManagementTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.GraduatesAcademicWorkTime < monthReportToAply.GraduatesAcademicWorkTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.GraduatesAcademicWorkTime > monthReportToAply.GraduatesAcademicWorkTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.PlasticPosesDemonstrationTime < monthReportToAply.PlasticPosesDemonstrationTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.PlasticPosesDemonstrationTime > monthReportToAply.PlasticPosesDemonstrationTime where contract identifier is {contract.ContractIdentifier}");
                if (untakenTimeIfRemoveTheReport.TestingEscortTime < monthReportToAply.TestingEscortTime)
                    throw new ArgumentException($"untakenTimeIfRemoveTheReport.TestingEscortTime > monthReportToAply.TestingEscortTime where contract identifier is {contract.ContractIdentifier}");
            }
            await _monthReportService.UpdateAsync(monthReportToAply);
        }

        public async Task BlockReport(int linkingPartID, int month, int year, int userID, CancellationToken token = default)
        {
            var user = await _accountService.FirstOrDefaultAsync(u => u.ID == userID, token) ?? throw new ObjectNotFoundException($"User wasn't found by ID = {userID}");
            if (!_accountService.IsAdmin(user))
            {
                throw new NoAccessException();
            }

            var report = await _monthReportService.FirstOrDefaultAsync(r => r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID) ?? throw new ObjectNotFoundException("r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID");
            report.BlockedByUser = user;
            report.BlockedByUserID = user.ID;
            await _monthReportService.UpdateAsync(report, token);
        }

        public async Task<IEnumerable<RelatedContractsWithReportsObject>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            return (await _monthReportService.GetReportsOnPeriodAsync(periodStart, periodEnd))
            .GroupBy(r => r.LinkingPart)
                .Select(m => new RelatedContractsWithReportsObject 
                { 
                    Contracts = m.Key.Assignments.Where(a => a.IsConfirmed && a.AssignmentDate <= periodEnd).ToList(), 
                    Reports = m.Where(r => (r.Month <= periodEnd.Month && r.Year <= periodEnd.Year) && (r.Month >= periodStart.Month && r.Year >= periodStart.Year)).ToList() 
                }).ToList(); 
        }

        public MonthReportsUntakenTimeModel GetUntakenTime(ContractLinkingPart contractLinkingPart, DateTime date, IEnumerable<(int year, int month)> exceptValuesWithKeys)
        {
            if (contractLinkingPart == null)
            {
                throw new ArgumentNullException(nameof(contractLinkingPart));
            }
            if (contractLinkingPart.Assignments == null || !contractLinkingPart.Assignments.Any()) 
            {
                throw new ArgumentException("contractLinkingPart.Assignments is null or empty");
            }

            var orderedContracts = contractLinkingPart.Assignments.Where(c=>c.IsConfirmed).OrderByDescending(c=>c.AssignmentDate);
            var contractOnDate = orderedContracts.FirstOrDefault(c => c.AssignmentDate <= date) ?? throw new ArgumentException($"Active contract on date {date} not found");
            var nextContract = orderedContracts.FirstOrDefault(c => c.AssignmentDate > contractOnDate.AssignmentDate);
            (int month, int year) = nextContract == null ? (contractOnDate.AssignmentDate.Month, contractOnDate.AssignmentDate.Year) : (nextContract.AssignmentDate.Month, nextContract.AssignmentDate.Year);
            var reports = contractLinkingPart.MonthReports.Where(c => (c.Year <= year && c.Month <= month) && !exceptValuesWithKeys.Contains((c.Year, c.Month))).ToList();
            var chk = reports.Sum((c) => c.PracticalClassesTime);
            var ret = new MonthReportsUntakenTimeModel
            {
                ContractID = contractOnDate.ID,
                ExceptForReportsWithKey = exceptValuesWithKeys,
                TestingEscortTime = contractOnDate.TestingEscortMaxTime - reports.Sum(c => c.TestingEscortTime),
                PlasticPosesDemonstrationTime = contractOnDate.PlasticPosesDemonstrationMaxTime - reports.Sum((c) => c.PlasticPosesDemonstrationTime),
                GraduatesAcademicWorkTime = contractOnDate.GraduatesAcademicWorkMaxTime - reports.Sum((c) => c.GraduatesAcademicWorkTime),
                GraduatesManagementTime = contractOnDate.GraduatesManagementMaxTime - reports.Sum((c) => c.GraduatesManagementTime),
                SECTime = contractOnDate.SECMaxTime - reports.Sum((c) => c.SECTime),
                DiplomasReviewsTime = contractOnDate.DiplomasReviewsMaxTime - reports.Sum((c) => c.DiplomasReviewsTime),
                DiplomasTime = contractOnDate.DiplomasMaxTime - reports.Sum((c) => c.DiplomasTime),
                InternshipsTime = contractOnDate.InternshipsMaxTime - reports.Sum((c) => c.InternshipsTime),
                TestsAndReferatsTime = contractOnDate.TestsAndReferatsMaxTime - reports.Sum((c) => c.TestsAndReferatsTime),
                InterviewsTime = contractOnDate.InterviewsMaxTime - reports.Sum((c) => c.InterviewsTime),
                LectionsTime = contractOnDate.LectionsMaxTime - reports.Sum((c) => c.LectionsTime),
                PracticalClassesTime = contractOnDate.PracticalClassesMaxTime - reports.Sum((c) => c.PracticalClassesTime),
                LaboratoryClassesTime = contractOnDate.LaboratoryClassesMaxTime - reports.Sum((c) => c.LaboratoryClassesTime),
                ConsultationsTime = contractOnDate.ConsultationsMaxTime - reports.Sum((c) => c.ConsultationsTime),
                OtherTeachingClassesTime = contractOnDate.OtherTeachingClassesMaxTime - reports.Sum((c) => c.OtherTeachingClassesTime),
                CreditsTime = contractOnDate.CreditsMaxTime - reports.Sum((c) => c.CreditsTime),
                ExamsTime = contractOnDate.ExamsMaxTime - reports.Sum((c) => c.ExamsTime),
                CourseProjectsTime = contractOnDate.CourseProjectsMaxTime - reports.Sum((c) => c.CourseProjectsTime)
            };
            return ret;
        }

        public async Task OnContractRemovedAsync(Contract contract)
        {
            if (contract == null || !contract.IsConfirmed)
            {
                return;
            }

            var linkingPart = await DbSet.Include(l => l.Assignments).Include(l => l.MonthReports).FirstOrDefaultAsync(l => l.Assignments.Contains(contract));
            if (linkingPart == null)
            {
                return;
            }

            if (linkingPart.SourceContractID == contract.ID)
            {
                DbSet.Remove(linkingPart);
            }

            var assigments = linkingPart.Assignments.OrderByDescending(a => a.AssignmentDate);
            var prevContract = assigments.FirstOrDefault(c => c.IsConfirmed && c.AssignmentDate < contract.AssignmentDate) ?? throw new Exception("prevContract not found");
            var nextContract = assigments.FirstOrDefault(c => c.IsConfirmed && c.AssignmentDate > contract.AssignmentDate);
            DateTime EndDate;

            if (nextContract == null)
            {
                EndDate = contract.PeriodEnd;
            }
            else 
            {
                EndDate = nextContract?.AssignmentDate.AddDays(-1) ?? throw new Exception("nextContract not found and here's an uttempt to get its pole");
            }
            var reports = linkingPart.MonthReports.Where(r => r.Year <= EndDate.Year && r.Month <= EndDate.Month).ToList();

            var untakenTime = GetUntakenTime(linkingPart, EndDate, Enumerable.Empty<(int year, int month)>());

            if (
                untakenTime.TimeSum < prevContract.TimeSum - contract.TimeSum
                || untakenTime.LectionsTime < contract.LectionsMaxTime - prevContract.LectionsMaxTime
                || untakenTime.PracticalClassesTime < contract.PracticalClassesMaxTime - prevContract.PracticalClassesMaxTime
                || untakenTime.LaboratoryClassesTime < contract.LaboratoryClassesMaxTime - prevContract.LaboratoryClassesMaxTime
                || untakenTime.ConsultationsTime < contract.ConsultationsMaxTime - prevContract.ConsultationsMaxTime
                || untakenTime.OtherTeachingClassesTime < prevContract.OtherTeachingClassesMaxTime - contract.OtherTeachingClassesMaxTime
                || untakenTime.CreditsTime < contract.CreditsMaxTime - prevContract.CreditsMaxTime
                || untakenTime.ExamsTime < contract.ExamsMaxTime - prevContract.ExamsMaxTime
                || untakenTime.CourseProjectsTime < contract.CourseProjectsMaxTime - prevContract.CourseProjectsMaxTime
                || untakenTime.InterviewsTime < contract.InterviewsMaxTime - prevContract.InterviewsMaxTime
                || untakenTime.TestsAndReferatsTime < contract.TestsAndReferatsMaxTime - prevContract.TestsAndReferatsMaxTime
                || untakenTime.InternshipsTime < contract.InternshipsMaxTime - prevContract.InternshipsMaxTime
                || untakenTime.DiplomasTime < contract.DiplomasMaxTime - prevContract.DiplomasMaxTime
                || untakenTime.DiplomasReviewsTime < contract.DiplomasReviewsMaxTime - prevContract.DiplomasReviewsMaxTime
                || untakenTime.SECTime < contract.SECMaxTime - prevContract.SECMaxTime
                || untakenTime.GraduatesManagementTime < contract.GraduatesManagementMaxTime - prevContract.GraduatesManagementMaxTime
                || untakenTime.GraduatesAcademicWorkTime < contract.GraduatesAcademicWorkMaxTime - prevContract.GraduatesAcademicWorkMaxTime
                || untakenTime.PlasticPosesDemonstrationTime < contract.PlasticPosesDemonstrationMaxTime - prevContract.PlasticPosesDemonstrationMaxTime
                || untakenTime.TestingEscortTime < contract.TestingEscortMaxTime - prevContract.TestingEscortMaxTime
                )
            {
                throw new ArgumentException("This contract's resource is used");
            }
        }

        public async Task<RelatedContractsWithReportsObject> GetFullData(int linkingPartID)
        {
            var part = await DbSet.Include(r => r.Assignments.Where(a => a.ConfirmedByUserID != null)).Include(r => r.MonthReports).FirstOrDefaultAsync(c => c.ID == linkingPartID) ?? throw new ObjectNotFoundException($"Object with id = {linkingPartID} not found");
            return new RelatedContractsWithReportsObject
            {
                Contracts = part.Assignments,
                Reports = part.MonthReports,
            };
        }

        public async Task UnBlockReport(int linkingPartID, int month, int year, int userID, CancellationToken token = default)
        {
            var user = await _accountService.FirstOrDefaultAsync(u => u.ID == userID, token) ?? throw new ObjectNotFoundException($"User wasn't found by ID = {userID}");
            if (!_accountService.IsAdmin(user))
            {
                throw new NoAccessException();
            }

            var report = await _monthReportService.FirstOrDefaultAsync(r => r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID, token) ?? throw new ObjectNotFoundException("r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID");
            report.BlockedByUserID = null; 
            await _monthReportService.UpdateAsync(report);
        }
    }
}
