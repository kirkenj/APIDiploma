using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Logic.Interfaces
{
    public interface IMonthReportService : IDbAccessServise<MonthReport>
    {
        public static IEnumerable<ValidationResult> Validate(MonthReport monthReport, ValidationContext validationContext)
        {
            if (monthReport.TimeSum < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.TimeSum)} < 0");
            }

            if (monthReport.TimeSum > 72)
            {
                yield return new ValidationResult($"{nameof(monthReport.TimeSum)} > 72");
            }

            if (monthReport.LectionsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.LectionsTime)} < 0");
            }

            if (monthReport.PracticalClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.PracticalClassesTime)} < 0");
            }

            if (monthReport.LaboratoryClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.LaboratoryClassesTime)} < 0");
            }

            if (monthReport.ConsultationsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.ConsultationsTime)} < 0");
            }

            if (monthReport.OtherTeachingClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.OtherTeachingClassesTime)} < 0");
            }

            if (monthReport.CreditsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.CreditsTime)} < 0");
            }

            if (monthReport.ExamsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.ExamsTime)} < 0");
            }

            if (monthReport.CourseProjectsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.CourseProjectsTime)} < 0");
            }

            if (monthReport.InterviewsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.InterviewsTime)} < 0");
            }

            if (monthReport.TestsAndReferatsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.TestsAndReferatsTime)} < 0");
            }

            if (monthReport.InternshipsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.InternshipsTime)} < 0");
            }

            if (monthReport.DiplomasTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.DiplomasTime)} < 0");
            }

            if (monthReport.DiplomasReviewsTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.DiplomasReviewsTime)} < 0");
            }

            if (monthReport.SECTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.SECTime)} < 0");
            }

            if (monthReport.GraduatesManagementTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.GraduatesManagementTime)} < 0");
            }

            if (monthReport.GraduatesAcademicWorkTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.GraduatesAcademicWorkTime)} < 0");
            }

            if (monthReport.PlasticPosesDemonstrationTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.PlasticPosesDemonstrationTime)} < 0");
            }

            if (monthReport.TestingEscortTime < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.TestingEscortTime)} < 0");
            }
        }
        public async Task<List<MonthReport>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd) => await DbSet.Include(m => m.LinkingPart).ThenInclude(l => l.Assignments).Where(m => (m.Year >= periodStart.Year && m.Month >= periodStart.Month) || (m.Year <= periodEnd.Year && m.Month <= periodEnd.Month)).ToListAsync();

    }
}
