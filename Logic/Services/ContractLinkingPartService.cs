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

        public async Task AddAsync(ContractLinkingPart entity, CancellationToken token)
        {
            DbSet.Add(entity);
            await SaveChangesAsync(token);
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

            var untakenTimeIfRemoveTheReport = await GetUntakenTimeAsync(report.LinkingPartID, new[] { (report.Year, report.Month) });
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

        public async Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int linkingPartID) => await _monthReportService.DBSet.Where(r => r.LinkingPartID == linkingPartID).ToListAsync();

        public async Task BlockReport(int linkingPartID, int year, int month, int userID, CancellationToken token = default)
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

        public async Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractID, IEnumerable<(int year, int month)> exceptValuesWithKeys)
        {
            var contact = await AssignmentsDBSet.FirstOrDefaultAsync(p => p.ID == contractID)
                ?? throw new KeyNotFoundException($"Contract wasn't found by ID = {contractID}");
            var clearedReports = (await GetMonthReportsAsync(contractID))
                .Where(r => !exceptValuesWithKeys.Any(e => e.year == r.Year && e.month == r.Year));

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

        public async Task<IEnumerable<(List<KeyValuePair<int, string>> relatedContractsIDs, List<MonthReport> monthReports)>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            var reports = await _monthReportService.DBSet.Include(m => m.LinkingPart).ThenInclude(l=>l.Assignments).Where(m => (m.Year >= periodStart.Year && m.Month >= periodStart.Month) || (m.Year <= periodEnd.Year && m.Month <= periodEnd.Month)).ToListAsync();
            return reports.GroupBy(r => r.LinkingPart).Select(m => (m.Key.Assignments.Where(a => a.IsConfirmed).Select(a => new KeyValuePair<int, string> (a.ID,a.ContractIdentifier)).ToList(), m.ToList())).ToList();
        }
    }
}
