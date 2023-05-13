using Database.Interfaces;
using Irony.Parsing;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces
{
    public interface IPeriodicValueService<TEntity, TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned> : IDbAccessServise<TEntity> 
        where TEntity : class, IPeriodicValueObject<TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned>
        where TAssignationType : class, IPeriodicValueAssignment<TAssignationValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        internal DbSet<TAssignationType> AssignmentsDBSet { get; }
    
        public async Task<IEnumerable<TAssignationType>> GetAssignmentForObject(TAssignationIDType objectID, CancellationToken token = default)
        {
            return await AssignmentsDBSet.Where(a => a.ObjectIdentifier.Equals(objectID)).ToListAsync(token);
        }

        public async Task<TAssignationType?> GetAssignmentOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return await AssignmentsDBSet.OrderByDescending(a => a.AssignmentDate).FirstOrDefaultAsync(a => a.AssignmentDate <= date && a.ObjectIdentifier.Equals(objectIDToFindPerVal), token);
        } 

        public async Task<TAssignationValueType?> GetAssignationValueOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return (await GetAssignmentOnDate(date, objectIDToFindPerVal, token))?.Value ?? null;
        } 

        public virtual async Task AddAssignmentAsync(TAssignationType assignation, CancellationToken token = default)
        {
            if (! await DbSet.AnyAsync(e => e.ID.Equals(assignation.ObjectIdentifier), token))
            {
                throw new ObjectNotFoundException($"{assignation.GetType().Name} with ID = {assignation.ObjectIdentifier} not found");
            }

            assignation.AssignmentDate = new DateTime(assignation.AssignmentDate.Year, assignation.AssignmentDate.Month, 1, 0, 0, 0);
            if (await AssignmentsDBSet.AnyAsync(a => a.AssignmentDate == assignation.AssignmentDate && assignation.ObjectIdentifier.Equals(a.ObjectIdentifier), token))
            {
                throw new ArgumentException($"{assignation.GetType().Name} with key [AssignationDate = {assignation.AssignmentDate}, ObjectIdentifier = {assignation.ObjectIdentifier}] already exists");
            }
            await AssignmentsDBSet.AddAsync(assignation, token);
            await SaveChangesAsync(token);
        }

        public virtual async Task EditAssignmentAsync(TAssignationIDType id, DateTime assignationActiveDate, TAssignationValueType newValue, DateTime? newAssignationDate, CancellationToken token = default)
        {
            var assignation = await GetAssignmentOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignationType).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
            assignation.Value = newValue;
            if (newAssignationDate.HasValue)
            {
                assignation.AssignmentDate = new DateTime(newAssignationDate.Value.Year, newAssignationDate.Value.Month, 1, 0, 0, 0);
            }

            await SaveChangesAsync(token);
        }

        public virtual async Task RemoveAssignmentAsync(TAssignationIDType id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            var assignation = await GetAssignmentOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignationType).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
            AssignmentsDBSet.Remove(assignation);
            await SaveChangesAsync(token);
        }

        public virtual async Task<IEnumerable<TAssignationType>> GetAllAssignmentsAsync(CancellationToken token = default)
        {
            return await AssignmentsDBSet.ToListAsync(token);
        }
    }
}