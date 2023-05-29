using Database.Interfaces;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces.Common
{
    public interface IPeriodicValueServiceWithEdit<TEntity, TAssignmentType, TAssignmentIDType, TAssignationValueType, TTypeBeingAssigned> : IPeriodicValueService<TEntity, TAssignmentType, TAssignmentIDType, TAssignationValueType, TTypeBeingAssigned>
        where TEntity : class, IPeriodicValueObject<TAssignmentType, TAssignmentIDType, TAssignationValueType, TTypeBeingAssigned>
        where TAssignmentType : class, IPeriodicValueAssignment<TAssignationValueType, TAssignmentIDType, TTypeBeingAssigned>
        where TAssignmentIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignmentIDType>
    {
        public virtual async Task AddAssignmentAsync(TAssignmentType assignment, CancellationToken token = default)
        {
            if (!await DbSet.AnyAsync(e => e.ID.Equals(assignment.ObjectIdentifier), token))
            {
                throw new ObjectNotFoundException($"{assignment.GetType().Name} with ID = {assignment.ObjectIdentifier} not found");
            }

            assignment.AssignmentDate = new DateTime(assignment.AssignmentDate.Year, assignment.AssignmentDate.Month, 1, 0, 0, 0);
            if (await AssignmentsDBSet.AnyAsync(a => a.AssignmentDate == assignment.AssignmentDate && assignment.ObjectIdentifier.Equals(a.ObjectIdentifier), token))
            {
                throw new ArgumentException($"{assignment.GetType().Name} with key [AssignmentDate = {assignment.AssignmentDate}, ObjectIdentifier = {assignment.ObjectIdentifier}] already exists");
            }
            await AssignmentsDBSet.AddAsync(assignment, token);
            await SaveChangesAsync(token);
        }

        public virtual async Task EditAssignmentAsync(TAssignmentIDType id, DateTime assignmentActiveDate, TAssignationValueType newValue, DateTime? newAssignmentDate, CancellationToken token = default)
        {
            var assignation = await GetAssignmentOnDate(assignmentActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignmentType).Name} not found with key [activeDate = {assignmentActiveDate}, ObjectID = {id}]");

            AssignmentsDBSet.Remove(assignation);
            await SaveChangesAsync(token);


            assignation.Value = newValue;
            if (newAssignmentDate.HasValue)
            {
                assignation.AssignmentDate = new DateTime(newAssignmentDate.Value.Year, newAssignmentDate.Value.Month, 1, 0, 0, 0);
            }

            AssignmentsDBSet.Add(assignation);
            await SaveChangesAsync(token);
        }

        public virtual async Task RemoveAssignmentAsync(TAssignmentIDType id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            var assignation = await GetAssignmentOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignmentType).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
            AssignmentsDBSet.Remove(assignation);
            await SaveChangesAsync(token);
        }

        public virtual async Task<IEnumerable<TAssignmentType>> GetAllAssignmentsAsync(CancellationToken token = default)
        {
            return await AssignmentsDBSet.ToListAsync(token);
        }
    }
}
