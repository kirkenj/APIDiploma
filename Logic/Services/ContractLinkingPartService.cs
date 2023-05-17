using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Interfaces.Common;
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
            var report = await _monthReportService.FirstOrDefaultAsync(r => r.LinkingPartID == monthReportToAply.LinkingPartID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year)
                ?? throw new ObjectNotFoundException($"Month report not found by key[ID = {monthReportToAply.LinkingPartID}, Month = {monthReportToAply.Month}, Year = {monthReportToAply.Year}]");
            if (report.IsBlocked)
            {
                throw new InvalidOperationException("This month report is blocked for edition");
            }

            var untakenTimeIfRemoveTheReport = await GetUntakenTimeAsync(report.LinkingPartID, new DateTime(report.Year, report.Month, 1), new[] { (report.Year, report.Month) });
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
            await _monthReportService.UpdateAsync(monthReportToAply);
        }

        public async Task BlockReport(int linkingPartID, int month, int year, int userID, CancellationToken token = default)
        {
            var user = await _accountService.FirstOrDefaultAsync(u => u.ID == userID, token) ?? throw new ObjectNotFoundException($"User wasn't found by ID = {userID}");
            if (!_accountService.IsAdmin(user))
            {
                throw new NoAccessException();
            }

            var report = await _monthReportService.DBSet.FirstOrDefaultAsync(r => r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID) ?? throw new ObjectNotFoundException("r.Month == month && r.Year == year && r.LinkingPartID == linkingPartID");
            report.BlockedByUserID = userID;
            await SaveChangesAsync(token);
        }

        public async Task<IEnumerable<RelatedContractsWithReportsObject>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            var reports = await _monthReportService.DBSet.Include(m => m.LinkingPart).ThenInclude(l=>l.Assignments).Where(m => (m.Year >= periodStart.Year && m.Month >= periodStart.Month) || (m.Year <= periodEnd.Year && m.Month <= periodEnd.Month)).ToListAsync();
            return reports.GroupBy(r => r.LinkingPart)
                .Select(m => new RelatedContractsWithReportsObject 
                { 
                    Contracts = m.Key.Assignments.Where(a => a.IsConfirmed && a.AssignmentDate <= periodEnd).ToList(), 
                    Reports = m.Where(r => (r.Month <= periodEnd.Month && r.Year <= periodEnd.Year) && (r.Month >= periodStart.Month && r.Year >= periodStart.Year)).ToList() 
                }).ToList(); 
        }

        public async Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractLinkingID, DateTime date, IEnumerable<(int year, int month)> exceptValuesWithKeys)
        {
            var linkingPart = await DbSet.Include(l => l.Assignments).Include(l => l.MonthReports).FirstAsync(l => l.ID == contractLinkingID);
            var orderedContracts = linkingPart.Assignments.Where(c=>c.IsConfirmed).OrderByDescending(c=>c.AssignmentDate);
            var contractOnDate = orderedContracts.FirstOrDefault(c => c.AssignmentDate <= date) ?? throw new ArgumentException($"Active contract on date {date} not found");
            var nextContract = orderedContracts.FirstOrDefault(c => c.AssignmentDate > contractOnDate.AssignmentDate);
            (int month, int year) = nextContract == null ? (contractOnDate.AssignmentDate.Month, contractOnDate.AssignmentDate.Year) : (nextContract.AssignmentDate.Month, nextContract.AssignmentDate.Year);
            var reports = linkingPart.MonthReports.Where(c => (c.Year <= year && c.Month <= month) && !exceptValuesWithKeys.Contains((c.Year, c.Month))).ToList();
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

            var untakenTime = await GetUntakenTimeAsync(linkingPart.ID, EndDate, Enumerable.Empty<(int year, int month)>());

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
    }
}
