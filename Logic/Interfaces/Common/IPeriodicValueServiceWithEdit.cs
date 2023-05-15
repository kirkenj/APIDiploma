using Database.Interfaces;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces.Common
{
    public interface IPeriodicValueServiceWithEdit<TEntity, TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned> : IPeriodicValueService<TEntity, TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned>
        where TEntity : class, IPeriodicValueObject<TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned>
        where TAssignationType : class, IPeriodicValueAssignment<TAssignationValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        public virtual async Task AddAssignmentAsync(TAssignationType assignation, CancellationToken token = default)
        {
            if (!await DbSet.AnyAsync(e => e.ID.Equals(assignation.ObjectIdentifier), token))
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
