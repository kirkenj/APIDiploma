using Database.Interfaces;
using Irony.Parsing;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces.Common
{
    public interface IPeriodicValueService<TEntity, TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned> : IDbAccessServise<TEntity>
        where TEntity : class, IPeriodicValueObject<TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned>
        where TAssignationType : class, IPeriodicValueAssignment<TAssignationValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        internal DbSet<TAssignationType> AssignmentsDBSet { get; }

        public async Task<IEnumerable<TAssignationType>> GetAssignmentsForObject(TAssignationIDType objectID, CancellationToken token = default)
        {
            return await AssignmentsDBSet.Where(a => a.ObjectIdentifier.Equals(objectID)).ToListAsync(token);
        }

        public async Task<TAssignationType?> GetAssignmentOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return await AssignmentsDBSet.OrderByDescending(a => a.AssignmentDate).FirstOrDefaultAsync(a => a.AssignmentDate <= date && a.ObjectIdentifier.Equals(objectIDToFindPerVal), token);
        }

        public async Task<TAssignationValueType?> GetAssignmentValueOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return (await GetAssignmentOnDate(date, objectIDToFindPerVal, token))?.Value ?? null;
        }
    }
}