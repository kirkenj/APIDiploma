using Database;
using Database.Entities;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Logic.Services
{
    public class MonthReportService : IMonthReportService
    {
        public DbSet<MonthReport> DbSet { get; private set; }

        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }

        public MonthReportService(AppDbContext dbContext)
        {
            DbSet = dbContext.Set<MonthReport>();
            SaveChangesAsync = dbContext.SaveChangesAsync;
        }

        public async Task UpdateAsync(MonthReport valueToAply, CancellationToken token = default)
        {
            var res = ValidateMonthReport(valueToAply);
            if (res.Any())
            {
                throw new ArgumentException(string.Join("\r\n", res));
            }

            var valueToUpdate = await DbSet.FirstOrDefaultAsync(m => m.LinkingPartID == valueToAply.LinkingPartID && m.Year == valueToAply.Year && m.Month == valueToAply.Month, cancellationToken: token)
                ?? throw new ObjectNotFoundException($"Month report not found by key [m.LinkingPartID == {valueToAply.LinkingPartID}, Year == {valueToAply.Month}, m.Month = {valueToAply.Month}]");
            
            
            
            
            DbSet.Remove(valueToUpdate);
            await SaveChangesAsync(token);
            DbSet.Add(valueToAply);
            await SaveChangesAsync(token);
        }


        public IEnumerable<ValidationResult> ValidateMonthReport(MonthReport monthReport)
        {
            if (monthReport.TimeSum < 0)
            {
                yield return new ValidationResult($"{nameof(monthReport.TimeSum)} < 0");
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

        public async Task AddAsync(MonthReport entity, bool SaveChanges = true, CancellationToken token = default)
        {
            await DbSet.AddAsync(entity, token);
            if (SaveChanges)
                await SaveChangesAsync(token);
        }

        public IQueryable<MonthReport> GetViaSelectionObject(MonthReportPartSelectObject? selectionObject, IQueryable<MonthReport> entities)
        {
            throw new NotImplementedException();
        }
    }
}
