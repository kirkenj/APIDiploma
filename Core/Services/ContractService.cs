using Database.Entities;
using Database.Interfaces;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task Add(Contract contract)
        {
            //var res = Validate(contract);
            //if (res.Any())
            //{
            //    throw new ArgumentException(string.Join("\n", res));
            //}

            _appDBContext.Contracts.Add(contract);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task ConfirmContract(Contract contract, User user)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (contract.IsConfirmed)
            {
                throw new ArgumentException("This contract is already confirmed");
            }

            if (!_accountService.IsAdmin(user))
            {
                throw new InvalidOperationException("This user has no rights to do this");
            }

            contract.ConfirmedByUserID = user.ID;
            GenerateMonthReportsForContract(contract);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contract>> GetAll(bool doIncludeAdditionalInfo = true)
        {
            throw new NotImplementedException();
        }
        

        public async Task<Contract?> GetById(int id)=> await _appDBContext.Contracts.Include(c=>c.User).Include(c=>c.Department).Include(c=>c.MonthReports).FirstOrDefaultAsync(c => c.ID == id);
    
        private static bool IsValidIfReplace(Contract contract, MonthReport monthReport)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (monthReport == null)
            {
                throw new ArgumentNullException(nameof(monthReport));
            }

            var reports = contract.MonthReports.ToList();
            var report = reports.FirstOrDefault(r => r.ContractID == monthReport.ContractID && r.Month == monthReport.Month && r.Year == monthReport.Year);
            if (report == null)
            {
                throw new ArgumentException(nameof(report));
            }

            reports.Remove(report);
            reports.Add(monthReport);
            if (reports.Sum(r=>r.TimeSum) > contract.TimeSum)
            {
                throw new Exception("reports.Sum(r=>r.TimeSum) > contract.TimeSum");
            }

            if (reports.Sum(r=>r.LectionsTime) > contract.LectionsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.LectionsTime) > contract.LectionsMaxTime");
            }

            if (reports.Sum(r=>r.PracticalClassesTime) > contract.PracticalClassesMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.PracticalClassesTime) > contract.PracticalClassesMaxTime");

            }

            if (reports.Sum(r=>r.LaboratoryClassesTime) > contract.LaboratoryClassesMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.LaboratoryClassesTime) > contract.LaboratoryClassesMaxTime");
            }

            if (reports.Sum(r=>r.ConsultationsTime) > contract.ConsultationsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.ConsultationsTime) > contract.ConsultationsMaxTime");
            }

            if (reports.Sum(r=>r.OtherTeachingClassesTime) > contract.OtherTeachingClassesMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.OtherTeachingClassesTime) > contract.OtherTeachingClassesMaxTime");
            }

            if (reports.Sum(r=>r.CreditsTime) > contract.CreditsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.CreditsTime) > contract.CreditsMaxTime");
            }

            if (reports.Sum(r=>r.ExamsTime) > contract.ExamsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.ExamsTime) > contract.ExamsMaxTime");
            }

            if (reports.Sum(r=>r.CourseProjectsTime) > contract.CourseProjectsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.CourseProjectsTime) > contract.CourseProjectsMaxTime");
            }

            if (reports.Sum(r=>r.InterviewsTime) > contract.InterviewsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.InterviewsTime) > contract.InterviewsMaxTime");
            }

            if (reports.Sum(r=>r.TestsAndReferatsTime) > contract.TestsAndReferatsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.TestsAndReferatsTime) > contract.TestsAndReferatsMaxTime");
            }

            if (reports.Sum(r=>r.InternshipsTime) > contract.InternshipsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.InternshipsTime) > contract.InternshipsMaxTime");
            }

            if (reports.Sum(r=>r.DiplomasTime) > contract.DiplomasMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.DiplomasTime) > contract.DiplomasMaxTime");
            }

            if (reports.Sum(r=>r.DiplomasReviewsTime) > contract.DiplomasReviewsMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.DiplomasReviewsTime) > contract.DiplomasReviewsMaxTime");
            }

            if (reports.Sum(r=>r.SECTime) > contract.SECMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.SECTime) > contract.SECMaxTime");
            }

            if (reports.Sum(r=>r.GraduatesManagementTime) > contract.GraduatesManagementMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.GraduatesManagementTime) > contract.GraduatesManagementMaxTime");
            }

            if (reports.Sum(r=>r.GraduatesAcademicWorkTime) > contract.GraduatesAcademicWorkMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.GraduatesAcademicWorkTime) > contract.GraduatesAcademicWorkMaxTime");
            }

            if (reports.Sum(r=>r.PlasticPosesDemonstrationTime) > contract.PlasticPosesDemonstrationMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.PlasticPosesDemonstrationTime) > contract.PlasticPosesDemonstrationMaxTime");
            }

            if (reports.Sum(r=>r.TestingEscortTime) > contract.TestingEscortMaxTime)
            {
                throw new Exception("reports.Sum(r=>r.TestingEscortTime) > contract.TestingEscortMaxTime");
            }

            return true;
        }


        public async Task UpdateMonthReport(MonthReport monthReport)
        {
            var res = monthReport.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(monthReport));
            if (res.Any())
            {
                throw new Exception(string.Join("\n", res));
            }

            var contract = await GetById(monthReport.ContractID);
            if (contract == null)
            {
                throw new Exception($"Not found {nameof(contract)}");
            }

            if (IsValidIfReplace(contract, monthReport))
            {
                var report = await _appDBContext.MonthReports.FirstOrDefaultAsync(r => r.ContractID == monthReport.ContractID && r.Month == monthReport.Month && r.Year == monthReport.Year);
                _appDBContext.MonthReports.Remove(report ?? throw new ArgumentNullException(nameof(report)));
                _appDBContext.MonthReports.Add(monthReport);
                await _appDBContext.SaveChangesAsync();
            }
        }

        private IEnumerable<MonthReport> GenerateMonthReportsForContract(Contract contract)
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

        public async Task<IEnumerable<Contract>> GetUserContracts(string UserName)=> await _appDBContext.Contracts.Include(c => c.MonthReports).Include(c => c.User).Where(c => c.User.Name == UserName).ToArrayAsync();

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (PeriodStart > PeriodEnd)
        //    {
        //        yield return new ValidationResult($"PeriodStart > PeriodEnd");
        //    }

        //    if (TimeSum < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(TimeSum)} < 0");
        //    }

        //    if (LectionsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(LectionsMaxTime)} < 0");
        //    }

        //    if (PracticalClassesMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(PracticalClassesMaxTime)} < 0");
        //    }

        //    if (LaboratoryClassesMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(LaboratoryClassesMaxTime)} < 0");
        //    }

        //    if (ConsultationsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(ConsultationsMaxTime)} < 0");
        //    }

        //    if (OtherTeachingClassesMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(OtherTeachingClassesMaxTime)} < 0");
        //    }

        //    if (CreditsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(CreditsMaxTime)} < 0");
        //    }

        //    if (ExamsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(ExamsMaxTime)} < 0");
        //    }

        //    if (CourseProjectsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(CourseProjectsMaxTime)} < 0");
        //    }

        //    if (InterviewsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(InterviewsMaxTime)} < 0");
        //    }

        //    if (TestsAndReferatsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(TestsAndReferatsMaxTime)} < 0");
        //    }

        //    if (InternshipsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(InternshipsMaxTime)} < 0");
        //    }

        //    if (DiplomasMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(DiplomasMaxTime)} < 0");
        //    }

        //    if (DiplomasReviewsMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(DiplomasReviewsMaxTime)} < 0");
        //    }

        //    if (SECMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(SECMaxTime)} < 0");
        //    }

        //    if (GraduatesManagementMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(GraduatesManagementMaxTime)} < 0");
        //    }

        //    if (GraduatesAcademicWorkMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(GraduatesAcademicWorkMaxTime)} < 0");
        //    }

        //    if (PlasticPosesDemonstrationMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(PlasticPosesDemonstrationMaxTime)} < 0");
        //    }

        //    if (TestingEscortMaxTime < 0)
        //    {
        //        yield return new ValidationResult($"{nameof(TestingEscortMaxTime)} < 0");
        //    }

        //    if (ParentContract != null)
        //    {

        //        if (TimeSum < ParentContract.TimeSum)
        //        {
        //            yield return new ValidationResult($"TimeSum < ParentContract.TimeSum");
        //        }

        //        if (LectionsMaxTime < ParentContract.LectionsMaxTime)
        //        {
        //            yield return new ValidationResult($"LectionsMaxTime < ParentContract.LectionsMaxTime");
        //        }

        //        if (PracticalClassesMaxTime < ParentContract.PracticalClassesMaxTime)
        //        {
        //            yield return new ValidationResult($"PracticalClassesMaxTime < ParentContract.PracticalClassesMaxTime");
        //        }

        //        if (LaboratoryClassesMaxTime < ParentContract.LaboratoryClassesMaxTime)
        //        {
        //            yield return new ValidationResult($"LaboratoryClassesMaxTime < ParentContract.LaboratoryClassesMaxTime");
        //        }

        //        if (ConsultationsMaxTime < ParentContract.ConsultationsMaxTime)
        //        {
        //            yield return new ValidationResult($"ConsultationsMaxTime < ParentContract.ConsultationsMaxTime");
        //        }

        //        if (OtherTeachingClassesMaxTime < ParentContract.OtherTeachingClassesMaxTime)
        //        {
        //            yield return new ValidationResult($"OtherTeachingClassesMaxTime < ParentContract.OtherTeachingClassesMaxTime");
        //        }

        //        if (CreditsMaxTime < ParentContract.CreditsMaxTime)
        //        {
        //            yield return new ValidationResult($"CreditsMaxTime < ParentContract.CreditsMaxTime");
        //        }

        //        if (ExamsMaxTime < ParentContract.ExamsMaxTime)
        //        {
        //            yield return new ValidationResult($"ExamsMaxTime < ParentContract.ExamsMaxTime");
        //        }

        //        if (CourseProjectsMaxTime < ParentContract.CourseProjectsMaxTime)
        //        {
        //            yield return new ValidationResult($"CourseProjectsMaxTime < ParentContract.CourseProjectsMaxTime");
        //        }

        //        if (InterviewsMaxTime < ParentContract.InterviewsMaxTime)
        //        {
        //            yield return new ValidationResult($"InterviewsMaxTime < ParentContract.InterviewsMaxTime");
        //        }

        //        if (TestsAndReferatsMaxTime < ParentContract.TestsAndReferatsMaxTime)
        //        {
        //            yield return new ValidationResult($"TestsAndReferatsMaxTime < ParentContract.TestsAndReferatsMaxTime");
        //        }

        //        if (InternshipsMaxTime < ParentContract.InternshipsMaxTime)
        //        {
        //            yield return new ValidationResult($"InternshipsMaxTime < ParentContract.InternshipsMaxTime");
        //        }

        //        if (DiplomasMaxTime < ParentContract.DiplomasMaxTime)
        //        {
        //            yield return new ValidationResult($"DiplomasMaxTime < ParentContract.DiplomasMaxTime");
        //        }

        //        if (DiplomasReviewsMaxTime < ParentContract.DiplomasReviewsMaxTime)
        //        {
        //            yield return new ValidationResult($"DiplomasReviewsMaxTime < ParentContract.DiplomasReviewsMaxTime");
        //        }

        //        if (SECMaxTime < ParentContract.SECMaxTime)
        //        {
        //            yield return new ValidationResult($"SECMaxTime < ParentContract.SECMaxTime");
        //        }

        //        if (GraduatesManagementMaxTime < ParentContract.GraduatesManagementMaxTime)
        //        {
        //            yield return new ValidationResult($"GraduatesManagementMaxTime < ParentContract.GraduatesManagementMaxTime");
        //        }

        //        if (GraduatesAcademicWorkMaxTime < ParentContract.GraduatesAcademicWorkMaxTime)
        //        {
        //            yield return new ValidationResult($"GraduatesAcademicWorkMaxTime < ParentContract.GraduatesAcademicWorkMaxTime");
        //        }

        //        if (PlasticPosesDemonstrationMaxTime < ParentContract.PlasticPosesDemonstrationMaxTime)
        //        {
        //            yield return new ValidationResult($"PlasticPosesDemonstrationMaxTime < ParentContract.PlasticPosesDemonstrationMaxTime");
        //        }

        //        if (TestingEscortMaxTime < ParentContract.TestingEscortMaxTime)
        //        {
        //            yield return new ValidationResult($"TestingEscortMaxTime < ParentContract.TestingEscortMaxTime");
        //        }
        //    }

    }
}
