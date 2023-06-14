using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Logic.Services
{
    public class ContractService : IContractService
    {
        public IAccountService _accountService { get; set; }
        public DbSet<Contract> DbSet { get; set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; set; }
        private readonly IContractLinkingPartService _contractLinkingPartService;
        private readonly IDepartmentService _departmentService;
        private readonly IContractTypeService _contractTypeService;


        public ContractService(IAppDBContext dBContext, IAccountService accountService, IContractLinkingPartService contractLinkingPartService, IDepartmentService departmentService, IContractTypeService contractTypeService)
        {
            DbSet = dBContext.Set<Contract>();
            SaveChangesAsync = dBContext.SaveChangesAsync;
            _accountService = accountService;
            _contractLinkingPartService = contractLinkingPartService;
            _departmentService = departmentService;
            _contractTypeService = contractTypeService;
        }

        public IQueryable<Contract> GetViaSelectionObject(ContractsSelectObject? selectionObject, IQueryable<Contract> contracts)
        {
            if (selectionObject == null)
            {
                return contracts;
            }

            if (selectionObject.IdentifierPart != null)
            {
                contracts = contracts.Where(c => c.ContractIdentifier.Contains(selectionObject.IdentifierPart));
            }

            if (selectionObject.DepartmentIDs != null)
            {
                contracts = contracts.Where(c => selectionObject.DepartmentIDs.Contains(c.DepartmentID));
            }

            if (selectionObject.IsConfirmed != null)
            {
                contracts = contracts.Where(c => c.IsConfirmed == selectionObject.IsConfirmed);
            }

            if (selectionObject.IsParent != null)
            {
                contracts = contracts.Where(c => (c.ParentContractID == null) == selectionObject.IsParent);
            }

            if (selectionObject.ContractTypeIDs != null)
            {
                contracts = contracts.Where(c => selectionObject.ContractTypeIDs.Contains(c.ContractTypeID));
            }

            if (selectionObject.UserIDs != null)
            {
                contracts = contracts.Where(c => selectionObject.UserIDs.Contains(c.UserID));
            }

            if (selectionObject.PeriodStartStartBound != null)
            {
                contracts = contracts.Where(c => c.PeriodStart >= selectionObject.PeriodStartStartBound);
            }

            if (selectionObject.PeriodStartEndBound != null)
            {
                contracts = contracts.Where(c => c.PeriodStart <= selectionObject.PeriodStartEndBound);
            }

            if (selectionObject.PeriodEndStartBound != null)
            {
                contracts = contracts.Where(c => c.PeriodEnd >= selectionObject.PeriodEndStartBound);
            }

            if (selectionObject.PeriodEndEndBound != null)
            {
                contracts = contracts.Where(c => c.PeriodEnd <= selectionObject.PeriodEndEndBound);
            }

            if (selectionObject.ConclusionDateEndBound != null)
            {
                contracts = contracts.Where(c => c.ConclusionDate <= selectionObject.ConclusionDateEndBound);
            }

            if (selectionObject.ConclusionDateStartBound != null)
            {
                contracts = contracts.Where(c => c.ConclusionDate >= selectionObject.ConclusionDateStartBound);
            }

            return contracts;
        }

        public IQueryable<KeyValuePair<Contract, bool>> GetContractHasChildKeyValuePair(ContractsSelectObject? selectionObject) => GetViaSelectionObject(selectionObject, DbSet)
            .Select(c => new KeyValuePair<Contract, bool>(c, c.ChildContracts.Any(q => q.IsConfirmed)));

        public async Task DeleteAsync(Contract entity, CancellationToken token = default)
        {
            DbSet.Remove(entity);
            if (entity.IsConfirmed)
            {
                await _contractLinkingPartService.OnContractRemovedAsync(entity);
            }

            await SaveChangesAsync(token);
        }

        public async Task AddAsync(Contract entity, bool SaveChanges = true, CancellationToken token = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (await DbSet.AnyAsync(c => c.ContractIdentifier == entity.ContractIdentifier && c.ConclusionDate == entity.ConclusionDate, token))
            {
                throw new ArgumentException("Contract identifier is not unique");
            }

            if (await _contractTypeService.FirstOrDefaultAsync(c => c.ID == entity.ContractTypeID, token) == null)
            {
                throw new ObjectNotFoundException($"Contract type with id = {entity.ContractTypeID} not found");
            }

            if (await _departmentService.FirstOrDefaultAsync(c => c.ID == entity.DepartmentID, token) == null)
            {
                throw new ObjectNotFoundException($"Department with id = {entity.DepartmentID} not found");
            }

            IEnumerable<ValidationResult> results;
            if (entity.ParentContractID != null)
            {
                if (await DbSet.AnyAsync(c => c.IsConfirmed && c.ParentContractID == entity.ParentContractID, token))
                {
                    throw new ArgumentException($"Contract with ID = {entity.ParentContractID} already has a confirmed child");
                }

                var parent = await DbSet.FirstOrDefaultAsync(c => c.ID == entity.ParentContractID, token) ?? throw new ObjectNotFoundException($"Object with ID = {entity.ParentContractID} not found");

                if (entity.PeriodStart.Month == parent.PeriodStart.Month && entity.PeriodStart.Year == parent.PeriodStart.Year)
                {
                    throw new ArgumentException("Child contract has same period start month with its parent");
                }

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
            if (SaveChanges)
            {
                await SaveChangesAsync(token);
            }
        }

        public async Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var contract = await DbSet
                .Include(c => c.LinkingPart)
                .ThenInclude(o => o.MonthReports)
                .FirstOrDefaultAsync(c => c.ID == contractID)
                ?? throw new ObjectNotFoundException($"Object with ID = {contractID} not found");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (!contract.IsConfirmed)
            {
                return Enumerable.Empty<MonthReport>();
            }

            return contract.LinkingPart?.MonthReports ?? throw new ArgumentNullException(nameof(contract.LinkingPart));
        }

        public async Task<string?> GetOwnersLoginAsync(int contractID) => (await DbSet.Include(c => c.User).FirstOrDefaultAsync(c => c.ID == contractID))?.User.Login ?? null;

        public async Task<IEnumerable<Contract>> GetRelatedContracts(Contract contract, CancellationToken token = default)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return contract.IsConfirmed ?
            (await DbSet.Include(c => c.LinkingPart).ThenInclude(l => l.Assignments).FirstAsync(c => c.ID == contract.ID, token))?.LinkingPart?.Assignments.Where(c => c.IsConfirmed) ?? throw new ArgumentNullException("LinkingPart")
            : Enumerable.Empty<Contract>();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public async Task<IEnumerable<RelatedContractsWithReportsObject>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd) => await _contractLinkingPartService.GetReportsOnPeriodAsync(periodStart, periodEnd);

        public async Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractID, IEnumerable<(int year, int month)> exceptValuesWithKeys, bool replaceNegativesWithZero = false) => await _contractLinkingPartService.GetContractsUntakenTimeAsync(contractID, exceptValuesWithKeys, replaceNegativesWithZero);

        public async Task OnObjectAboutToBeConfirmedAsync(Contract entity, CancellationToken token = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.AssignmentDate = new DateTime(entity.PeriodStart.Year, entity.PeriodStart.Month, 1);
            if (entity.ParentContractID != null)
            {
                if (await DbSet.AnyAsync(c => c.IsConfirmed && c.ParentContractID == entity.ParentContractID, token))
                {
                    throw new ArgumentException($"Contract with ID = {entity.ParentContractID} already has a confirmed child");
                }

                var parentContract = await DbSet.FirstAsync(c => c.ID == entity.ParentContractID, token);
                var relCheck = ValidateParentRelation(entity, parentContract);
                if (relCheck.Any())
                {
                    throw new ArgumentException(string.Join("\n", relCheck));
                }

                entity.LinkingPartID = parentContract.LinkingPartID;
                DbSet.RemoveRange(DbSet.Where(c => c.ID != entity.ID && c.ParentContractID == entity.ParentContractID));
            }
            else
            {
                var linkingPart = new ContractLinkingPart
                {
                    SourceContractID = entity.ID
                };

                entity.LinkingPart = linkingPart;
                var dateStart = new DateTime(entity.PeriodStart.Year, entity.PeriodStart.Month, 1);
                var dateEnd = new DateTime(entity.PeriodEnd.Year, entity.PeriodEnd.Month, 1);
                var reports = new List<MonthReport>();
                while (dateStart <= dateEnd)
                {
                    reports.Add(new MonthReport { LinkingPart = linkingPart, LinkingPartID = linkingPart.ID, Month = dateStart.Month, Year = dateStart.Year });
                    dateStart = dateStart.AddMonths(1);
                }

                linkingPart.MonthReports = reports;
                await _contractLinkingPartService.AddAsync(linkingPart, false, token);
            }
        }

        public async Task UpdateAsync(Contract valueToAply, CancellationToken token = default)
        {
            var record = await DbSet.FirstOrDefaultAsync(v => v.ID == valueToAply.ID, token) ?? throw new ObjectNotFoundException($"Object with ID = {valueToAply.ID} not found");
            if (record.IsConfirmed)
            {
                throw new ArgumentException("This contract is confirmed");
            }


            if (valueToAply == null)
            {
                throw new ArgumentNullException(nameof(valueToAply));
            }

            if (await DbSet.AnyAsync(c => c.ContractIdentifier == valueToAply.ContractIdentifier && c.ConclusionDate == valueToAply.ConclusionDate && c.ID != valueToAply.ID, token))
            {
                throw new ArgumentException("Contract identifier is not unique");
            }

            if (await _contractTypeService.FirstOrDefaultAsync(c => c.ID == valueToAply.ContractTypeID, token) == null)
            {
                throw new ObjectNotFoundException($"Contract type with id = {valueToAply.ContractTypeID} not found");
            }

            if (await _departmentService.FirstOrDefaultAsync(c => c.ID == valueToAply.DepartmentID, token) == null)
            {
                throw new ObjectNotFoundException($"Department with id = {valueToAply.DepartmentID} not found");
            }

            IEnumerable<ValidationResult> results;

            if (valueToAply.ParentContractID != null)
            {
                if (await DbSet.AnyAsync(c => c.IsConfirmed && c.ParentContractID == valueToAply.ParentContractID, token))
                {
                    throw new ArgumentException($"Contract with ID = {valueToAply.ParentContractID} already has a confirmed child");
                }

                var parent = await DbSet.FirstOrDefaultAsync(c => c.ID == valueToAply.ParentContractID, token) ?? throw new ObjectNotFoundException($"Object with ID = {valueToAply.ParentContractID} not found");

                if (valueToAply.PeriodStart.Month == parent.PeriodStart.Month && valueToAply.PeriodStart.Year == parent.PeriodStart.Year)
                {
                    throw new ArgumentException("Child contract has same period start month with its parent");
                }

                valueToAply.PeriodEnd = parent.PeriodEnd;
                valueToAply.DepartmentID = parent.DepartmentID;
                valueToAply.ContractTypeID = parent.ContractTypeID;
                valueToAply.PeriodEnd = parent.PeriodEnd;
                results = ValidateParentRelation(valueToAply, parent);
                if (results.Any())
                {
                    throw new ArgumentException(string.Join("\n", results));
                }
            }

            results = ValidateOnCreate(valueToAply);
            if (results.Any())
            {
                throw new ArgumentException(string.Join("\n", results));
            }

            DbSet.Remove(record);
            await SaveChangesAsync(token);
            valueToAply.ID = record.ID;
            DbSet.Add(valueToAply);
            await SaveChangesAsync(token);
        }

        public async Task UpdateMonthReport(MonthReport monthReport) => await _contractLinkingPartService.UpdateMonthReport(monthReport);

        private static IEnumerable<ValidationResult> ValidateOnCreate(Contract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (contract.ConclusionDate > contract.PeriodStart)
            {
                yield return new ValidationResult($"{nameof(contract.ConclusionDate)} > {nameof(contract.PeriodStart)}");
            }

            if (contract.ParentContractID != null && contract.ParentContractID == contract.ID)
            {
                yield return new ValidationResult($"{nameof(contract.ParentContractID)} = {nameof(contract.ID)}");
            }

            if (contract.PeriodStart >= contract.PeriodEnd)
            {
                yield return new ValidationResult($"{nameof(contract.PeriodStart)} >= {nameof(contract.PeriodEnd)}");
            }

            if (contract.TimeSum <= 0)
            {
                yield return new ValidationResult($"{nameof(contract.TimeSum)} <= 0");
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

            if (child.ParentContractID != parent.ID)
            {
                throw new ArgumentException("ParentContractID != ParentContract.ID");
            }

            if (child.ConclusionDate < parent.ConclusionDate)
            {
                yield return new ValidationResult("ConclusionDate < ParentContract.ConclusionDate");
            }

            if (child.ContractTypeID != parent.ContractTypeID)
            {
                yield return new ValidationResult("ContractTypeID != ParentContract.ContractTypeID");
            }

            if (child.PeriodStart <= parent.PeriodStart || child.PeriodStart > parent.PeriodEnd)
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

        public async Task BlockReport(int linkingPartID, int month, int year, int userID) => await _contractLinkingPartService.BlockReport(linkingPartID, month, year, userID);

        public async Task<RelatedContractsWithReportsObject> GetFullData(int contractID)
        {
            var contract = await DbSet.FirstOrDefaultAsync(c => c.ID == contractID) ?? throw new ObjectNotFoundException($"Contract with ID = {contractID} not found");
            if (!contract.IsConfirmed)
            {
                return new RelatedContractsWithReportsObject()
                {
                    Contracts = new List<Contract> { contract }
                };
            }

            return await _contractLinkingPartService.GetFullData(contract.LinkingPartID ?? throw new Exception("contract.LinkingPartID is null"));
        }

        public async Task UnBlockReport(int linkingPartID, int month, int year, int userID) => await _contractLinkingPartService.UnBlockReport(linkingPartID, month, year, userID);

        public async Task<MonthReport?> GetReport(int linkingPartID, int month, int year) => await _contractLinkingPartService.GetReport(linkingPartID, month, year);

        public async Task<MonthReportsUntakenTimeModel> GetMaxValuesForReport(int linkingPartId, int repYear, int repMonth) => await _contractLinkingPartService.GetMaxValuesForReport(linkingPartId, repYear, repMonth);
    }
}