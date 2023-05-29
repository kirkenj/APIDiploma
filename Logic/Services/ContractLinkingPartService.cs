using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class ContractLinkingPartService : IContractLinkingPartService
    {
        private readonly IMonthReportService _monthReportService;
        private readonly IAccountService _accountService;
        public DbSet<Contract> AssignmentsDBSet { get; set; }
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
            var linkingPart = await DbSet.Include(l => l.MonthReports).Include(l => l.Assignments.Where(a => a.IsConfirmed)).FirstOrDefaultAsync(l => l.MonthReports.Any(r => r.LinkingPartID == monthReportToAply.LinkingPartID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year))
                ?? throw new ObjectNotFoundException($"Month report not found by key[ID = {monthReportToAply.LinkingPartID}, Month = {monthReportToAply.Month}, Year = {monthReportToAply.Year}]");
            var report = linkingPart.MonthReports.First(r => r.LinkingPartID == monthReportToAply.LinkingPartID && r.Month == monthReportToAply.Month && r.Year == monthReportToAply.Year);

            if (report.IsBlocked)
            {
                throw new InvalidOperationException("This month report is blocked for edition");
            }

            var reportDate = new DateTime(monthReportToAply.Year, monthReportToAply.Month, 1);
            var contract = linkingPart.Assignments.OrderByDescending(c => c.AssignmentDate).First(c => c.AssignmentDate <= reportDate);
            var untakenTimeIfRemoveTheReport = await GetUntakenTimeAsync(contract.ID, new[] { (report.Year, report.Month) });
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
                await SaveChangesAsync(CancellationToken.None);
                return;
            }

            var assigments = linkingPart.Assignments.OrderByDescending(a => a.AssignmentDate);
            var prevContract = assigments.FirstOrDefault(c => c.IsConfirmed && c.AssignmentDate < contract.AssignmentDate) ?? throw new Exception("prevContract not found");
            var untakenTime = await GetUntakenTimeAsync(prevContract.ID, Enumerable.Empty<(int year, int month)>());

            if (
                untakenTime.TimeSum < 0
                || untakenTime.LectionsTime < 0
                || untakenTime.PracticalClassesTime < 0
                || untakenTime.LaboratoryClassesTime < 0
                || untakenTime.ConsultationsTime < 0
                || untakenTime.OtherTeachingClassesTime < 0
                || untakenTime.CreditsTime < 0
                || untakenTime.ExamsTime < 0
                || untakenTime.CourseProjectsTime < 0
                || untakenTime.InterviewsTime < 0
                || untakenTime.TestsAndReferatsTime < 0
                || untakenTime.InternshipsTime < 0
                || untakenTime.DiplomasTime < 0
                || untakenTime.DiplomasReviewsTime < 0
                || untakenTime.SECTime < 0
                || untakenTime.GraduatesManagementTime < 0
                || untakenTime.GraduatesAcademicWorkTime < 0
                || untakenTime.PlasticPosesDemonstrationTime < 0
                || untakenTime.TestingEscortTime < 0
                )
            {
                throw new ArgumentException("This contract's resource is used");
            }
        }

        public async Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractID, IEnumerable<(int year, int month)> exceptValuesWithKeys)
        {
            var linkingPart = await DbSet.Include(c => c.Assignments).Include(l => l.MonthReports).FirstOrDefaultAsync(c => c.Assignments.Any(a => a.IsConfirmed && a.ID == contractID)) ?? throw new ObjectNotFoundException($"Object with ID = {contractID} not found or not confirmed");
            var contract = linkingPart.Assignments.First(a => a.ID == contractID);
            var reports = linkingPart.MonthReports.Where(c => !exceptValuesWithKeys.Contains((c.Year, c.Month))).ToList() ?? throw new Exception("contract.LinkingPart is null");
            var ret = new MonthReportsUntakenTimeModel
            {
                ContractID = contract.ID,
                ExceptForReportsWithKey = exceptValuesWithKeys,
                TestingEscortTime = contract.TestingEscortMaxTime - reports.Sum(c => c.TestingEscortTime),
                PlasticPosesDemonstrationTime = contract.PlasticPosesDemonstrationMaxTime - reports.Sum((c) => c.PlasticPosesDemonstrationTime),
                GraduatesAcademicWorkTime = contract.GraduatesAcademicWorkMaxTime - reports.Sum((c) => c.GraduatesAcademicWorkTime),
                GraduatesManagementTime = contract.GraduatesManagementMaxTime - reports.Sum((c) => c.GraduatesManagementTime),
                SECTime = contract.SECMaxTime - reports.Sum((c) => c.SECTime),
                DiplomasReviewsTime = contract.DiplomasReviewsMaxTime - reports.Sum((c) => c.DiplomasReviewsTime),
                DiplomasTime = contract.DiplomasMaxTime - reports.Sum((c) => c.DiplomasTime),
                InternshipsTime = contract.InternshipsMaxTime - reports.Sum((c) => c.InternshipsTime),
                TestsAndReferatsTime = contract.TestsAndReferatsMaxTime - reports.Sum((c) => c.TestsAndReferatsTime),
                InterviewsTime = contract.InterviewsMaxTime - reports.Sum((c) => c.InterviewsTime),
                LectionsTime = contract.LectionsMaxTime - reports.Sum((c) => c.LectionsTime),
                PracticalClassesTime = contract.PracticalClassesMaxTime - reports.Sum((c) => c.PracticalClassesTime),
                LaboratoryClassesTime = contract.LaboratoryClassesMaxTime - reports.Sum((c) => c.LaboratoryClassesTime),
                ConsultationsTime = contract.ConsultationsMaxTime - reports.Sum((c) => c.ConsultationsTime),
                OtherTeachingClassesTime = contract.OtherTeachingClassesMaxTime - reports.Sum((c) => c.OtherTeachingClassesTime),
                CreditsTime = contract.CreditsMaxTime - reports.Sum((c) => c.CreditsTime),
                ExamsTime = contract.ExamsMaxTime - reports.Sum((c) => c.ExamsTime),
                CourseProjectsTime = contract.CourseProjectsMaxTime - reports.Sum((c) => c.CourseProjectsTime)
            };
            #region comment
            //ret.TestingEscortTime = ret.TestingEscortTime < 0 ? 0 : ret.TestingEscortTime;
            //ret.PlasticPosesDemonstrationTime = ret.PlasticPosesDemonstrationTime < 0 ? 0 : ret.PlasticPosesDemonstrationTime;
            //ret.GraduatesAcademicWorkTime = ret.GraduatesAcademicWorkTime < 0 ? 0 : ret.GraduatesAcademicWorkTime;
            //ret.GraduatesManagementTime = ret.GraduatesManagementTime < 0 ? 0 : ret.GraduatesManagementTime;
            //ret.SECTime = ret.SECTime < 0 ? 0 : ret.SECTime;
            //ret.DiplomasReviewsTime = ret.DiplomasReviewsTime < 0 ? 0 : ret.DiplomasReviewsTime;
            //ret.DiplomasTime = ret.DiplomasTime < 0 ? 0 : ret.DiplomasTime;
            //ret.InternshipsTime = ret.InternshipsTime < 0 ? 0 : ret.InternshipsTime;
            //ret.TestsAndReferatsTime = ret.TestsAndReferatsTime < 0 ? 0 : ret.TestsAndReferatsTime;
            //ret.InterviewsTime = ret.InterviewsTime < 0 ? 0 : ret.InterviewsTime;
            //ret.LectionsTime = ret.LectionsTime < 0 ? 0 : ret.LectionsTime;
            //ret.PracticalClassesTime = ret.PracticalClassesTime < 0 ? 0 : ret.PracticalClassesTime;
            //ret.LaboratoryClassesTime = ret.LaboratoryClassesTime < 0 ? 0 : ret.LaboratoryClassesTime;
            //ret.ConsultationsTime = ret.ConsultationsTime < 0 ? 0 : ret.ConsultationsTime;
            //ret.OtherTeachingClassesTime = ret.OtherTeachingClassesTime < 0 ? 0 : ret.OtherTeachingClassesTime;
            //ret.CreditsTime = ret.CreditsTime < 0 ? 0 : ret.CreditsTime;
            //ret.ExamsTime = ret.ExamsTime < 0 ? 0 : ret.ExamsTime;
            //ret.CourseProjectsTime = ret.CourseProjectsTime < 0 ? 0 : ret.CourseProjectsTime;
            #endregion
            return ret;
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
