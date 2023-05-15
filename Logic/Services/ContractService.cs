using Database.Entities;
using Database.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Logic.Interfaces;
using Logic.Interfaces.Common;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Logic.Exceptions;

namespace Logic.Services
{
    public class ContractService : IContractService
    {
        public IAccountService _accountService {get;set;}
        public DbSet<Contract> DbSet { get; set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; set; }
        public IContractLinkingPartService _contractLinkingPartService { get; set; }

        public ContractService(IAppDBContext dBContext, IAccountService accountService, IContractLinkingPartService contractLinkingPartService)
        {
            DbSet = dBContext.Set<Contract>();
            SaveChangesAsync = dBContext.SaveChangesAsync;
            _accountService = accountService;
            _contractLinkingPartService = contractLinkingPartService;

        }

        public async Task AddAsync(Contract entity, CancellationToken token = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (await DbSet.AnyAsync(c => c.ContractIdentifier == entity.ContractIdentifier, token))
            {
                throw new ArgumentException("Contract identifier is not unique");
            }

            IEnumerable<ValidationResult> results;
            if (entity.ParentContractID != null)
            {
                var parent = await DbSet.FirstOrDefaultAsync(c => c.ID == entity.ParentContractID, token) ?? throw new ObjectNotFoundException($"Object with ID = {entity.ParentContractID} not found");
                entity.PeriodEnd = parent.PeriodEnd;
                entity.DepartmentID = parent.DepartmentID;
                entity.ContractTypeID = parent.ContractTypeID;
                results = ValidateParentRelation(entity, parent);
                if (results.Any())
                {
                    throw new ArgumentException(string.Join("\n", results));
                }
            }
            else
            {
                results = ValidateOnCreate(entity);
                if (results.Any())
                {
                    throw new ArgumentException(string.Join("\n", results));
                }
            }

            DbSet.Add(entity);
            await SaveChangesAsync(token);
        }

        public async Task<IEnumerable<Contract>> GetAll() => await DbSet.ToListAsync();

        public async Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID)
        {
            var contract = await DbSet.Include(c => c.LinkingPart).ThenInclude(o=>o.MonthReports).FirstOrDefaultAsync(c => c.ID == contractID) ?? throw new ObjectNotFoundException($"Object with ID = {contractID} not found");
            if (!contract.IsConfirmed)
            {
                return Enumerable.Empty<MonthReport>();
            }

            return contract.LinkingPart.MonthReports;
        }

        public async Task<string?> GetOwnersLoginAsync(int contractID) => (await DbSet.Include(c => c.User).FirstOrDefaultAsync(c => c.ID == contractID))?.User.Login ?? null;

        public async Task<IEnumerable<Contract>> GetRelatedContracts(Contract contract, CancellationToken token = default) => contract.IsConfirmed ? 
            (await DbSet.Include(c => c.LinkingPart).ThenInclude(o => o.Assignments).FirstAsync(c => c.ID == contract.ID, token)).LinkingPart.Assignments.Where(c => c.IsConfirmed) 
            : Enumerable.Empty<Contract>();

        public async Task<IEnumerable<(List<KeyValuePair<int, string>> relatedContractsIDs, List<MonthReport> monthReports)>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd) => await _contractLinkingPartService.GetReportsOnPeriodAsync(periodStart, periodEnd);

        public async Task<MonthReportsUntakenTimeModel> GetUntakenTimeOnDateAsync(int contractID, DateTime date, IEnumerable<(int year, int month)> exceptValuesWithKeys)
        {
            throw new NotImplementedException();
            var contract = await DbSet.FirstOrDefaultAsync(c => c.ID == contractID) ?? throw new ObjectNotFoundException($"Object with ID = {contractID} not found");
            if (!contract.IsConfirmed)
            {
                throw new ArgumentException("This contract is not confirmed");
            }

            var linkingPart = contract.LinkingPartID;
            //_contractLinkingPartService.GetUntakenTimeAsync
        }

        public async Task OnObjectConfirmedAsync(Contract entity, CancellationToken token = default)
        {
            if (entity == null) 
            {
                throw new ArgumentNullException(nameof(entity)); 
            }

            entity.AssignmentDate = new DateTime (entity.PeriodStart.Year, entity.PeriodStart.Month, 1);
            if (entity.ParentContractID != null)
            {
                var parentContract = await DbSet.FirstAsync(c => c.ID == entity.ParentContractID, token);
                var relCheck = ValidateParentRelation(entity, parentContract);
                if (relCheck.Any())
                {
                    throw new ArgumentException(string.Join("\n", relCheck));
                }

                parentContract.ChildContractID = entity.ID;
                parentContract.ChildContract = entity;
                entity.LinkingPartID = parentContract.LinkingPartID;
            }
            else
            {
                var linkingPart = new ContractLinkingPart
                {
                    SourceContractID = entity.ID
                };

                entity.PeriodEnd = entity.PeriodEnd ?? throw new ArgumentNullException($"{nameof(entity.PeriodEnd)}");
                var dateStart = new DateTime(entity.PeriodStart.Year, entity.PeriodStart.Month, 1);
                var dateEnd = new DateTime(entity.PeriodEnd.Value.Year, entity.PeriodEnd.Value.Month, 1);
                var reports = new List<MonthReport>();
                while (dateStart <= dateEnd)
                {
                    reports.Add(new MonthReport { LinkingPart = linkingPart, LinkingPartID = linkingPart.ID, Month = dateStart.Month, Year = dateStart.Year });
                    dateStart = dateStart.AddMonths(1);
                }
                linkingPart.MonthReports = reports;
                await _contractLinkingPartService.AddAsync(linkingPart, token);
            }

            await SaveChangesAsync(token);
        }

        public async Task UpdateAsync(Contract valueToAply, CancellationToken token = default)
        {
            var record = await DbSet.FirstOrDefaultAsync(v => v.ID == valueToAply.ID, token) ?? throw new ObjectNotFoundException($"Object with ID = {valueToAply.ID} not found");
            if (record.IsConfirmed)
            {
                throw new ArgumentException("This contract is confirmed");
            }

            DbSet.Remove(record);
            await AddAsync(valueToAply, token);
        }

        public Task UpdateMonthReport(MonthReport monthReport)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<ValidationResult> ValidateOnCreate(Contract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (contract.PeriodEnd is null)
            {
                throw new ArgumentNullException(nameof(contract.PeriodEnd));
            }

            if (contract.PeriodStart >= contract.PeriodEnd.Value)
            {
                yield return new ValidationResult($"{nameof(contract.PeriodStart)} >= {nameof(contract.PeriodEnd)}");
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

        private static IEnumerable<ValidationResult> ValidateParentRelation(Contract child, Contract parent)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (!parent.IsConfirmed)
            {
                yield return new ValidationResult("Parent contract is not confirmed");
            }

            if (parent.ChildContractID != null && parent.ChildContractID != child.ID)
            {
                yield return new ValidationResult("parent.ChildContractID != child.ID");
            }

            if (child.ParentContractID != parent.ID)
            {
                throw new ArgumentException("ParentContractID != ParentContract.ID");
            }

            if (child.ContractTypeID != parent.ContractTypeID)
            {
                yield return new ValidationResult("ContractTypeID != ParentContract.ContractTypeID");
            }

            if (child.PeriodStart <= parent.PeriodStart || child.PeriodStart >= parent.PeriodEnd)
            {
                yield return new ValidationResult("child.PeriodStart <= parent.PeriodStart || child.PeriodStart >= parent.PeriodEnd");
            }

            if (child.PeriodEnd != parent.PeriodEnd)
            {
                yield return new ValidationResult("child.PeriodEnd != parent.PeriodEnd");
            }

            if (child.UserID != parent.UserID)
            {
                yield return new ValidationResult($"UserID != ParentContract.UserID");
            }

            if (child.TimeSum <= parent.TimeSum)
            {
                yield return new ValidationResult($"TimeSum <= ParentContract.TimeSum");
            }

            if (child.LectionsMaxTime < parent.LectionsMaxTime)
            {
                yield return new ValidationResult($"LectionsMaxTime < ParentContract.LectionsMaxTime");
            }

            if (child.PracticalClassesMaxTime < parent.PracticalClassesMaxTime)
            {
                yield return new ValidationResult($"PracticalClassesMaxTime < ParentContract.PracticalClassesMaxTime");
            }

            if (child.LaboratoryClassesMaxTime < parent.LaboratoryClassesMaxTime)
            {
                yield return new ValidationResult($"LaboratoryClassesMaxTime < ParentContract.LaboratoryClassesMaxTime");
            }

            if (child.ConsultationsMaxTime < parent.ConsultationsMaxTime)
            {
                yield return new ValidationResult($"ConsultationsMaxTime < ParentContract.ConsultationsMaxTime");
            }

            if (child.OtherTeachingClassesMaxTime < parent.OtherTeachingClassesMaxTime)
            {
                yield return new ValidationResult($"OtherTeachingClassesMaxTime < ParentContract.OtherTeachingClassesMaxTime");
            }

            if (child.CreditsMaxTime < parent.CreditsMaxTime)
            {
                yield return new ValidationResult($"CreditsMaxTime < ParentContract.CreditsMaxTime");
            }

            if (child.ExamsMaxTime < parent.ExamsMaxTime)
            {
                yield return new ValidationResult($"ExamsMaxTime < ParentContract.ExamsMaxTime");
            }

            if (child.CourseProjectsMaxTime < parent.CourseProjectsMaxTime)
            {
                yield return new ValidationResult($"CourseProjectsMaxTime < ParentContract.CourseProjectsMaxTime");
            }

            if (child.InterviewsMaxTime < parent.InterviewsMaxTime)
            {
                yield return new ValidationResult($"InterviewsMaxTime < ParentContract.InterviewsMaxTime");
            }

            if (child.TestsAndReferatsMaxTime < parent.TestsAndReferatsMaxTime)
            {
                yield return new ValidationResult($"TestsAndReferatsMaxTime < ParentContract.TestsAndReferatsMaxTime");
            }

            if (child.InternshipsMaxTime < parent.InternshipsMaxTime)
            {
                yield return new ValidationResult($"InternshipsMaxTime < ParentContract.InternshipsMaxTime");
            }

            if (child.DiplomasMaxTime < parent.DiplomasMaxTime)
            {
                yield return new ValidationResult($"DiplomasMaxTime < ParentContract.DiplomasMaxTime");
            }

            if (child.DiplomasReviewsMaxTime < parent.DiplomasReviewsMaxTime)
            {
                yield return new ValidationResult($"DiplomasReviewsMaxTime < ParentContract.DiplomasReviewsMaxTime");
            }

            if (child.SECMaxTime < parent.SECMaxTime)
            {
                yield return new ValidationResult($"SECMaxTime < ParentContract.SECMaxTime");
            }

            if (child.GraduatesManagementMaxTime < parent.GraduatesManagementMaxTime)
            {
                yield return new ValidationResult($"GraduatesManagementMaxTime < ParentContract.GraduatesManagementMaxTime");
            }

            if (child.GraduatesAcademicWorkMaxTime < parent.GraduatesAcademicWorkMaxTime)
            {
                yield return new ValidationResult($"GraduatesAcademicWorkMaxTime < ParentContract.GraduatesAcademicWorkMaxTime");
            }

            if (child.PlasticPosesDemonstrationMaxTime < parent.PlasticPosesDemonstrationMaxTime)
            {
                yield return new ValidationResult($"PlasticPosesDemonstrationMaxTime < ParentContract.PlasticPosesDemonstrationMaxTime");
            }

            if (child.TestingEscortMaxTime < parent.TestingEscortMaxTime)
            {
                yield return new ValidationResult($"TestingEscortMaxTime < ParentContract.TestingEscortMaxTime");
            }
        }
    }
}