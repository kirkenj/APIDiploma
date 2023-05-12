using Database.Interfaces;
using Irony.Parsing;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces
{
    public interface IPeriodicValueService<TEntity, TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned> : IDbAccessServise<TEntity> 
        where TEntity : class, IPeriodicValueObject<TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned>
        where TAssignationType : class, IPeriodicValueAssignation<TAssignationValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        public DbSet<TAssignationType> AssignationsDBSet { get; }
    
        public async Task<IEnumerable<TAssignationType>> GetAssignationsForObject(TAssignationIDType objectID, CancellationToken token = default)
        {
            return await AssignationsDBSet.Where(a => a.ObjectIdentifier.Equals(objectID)).ToListAsync(token);
        }

        public async Task<TAssignationType?> GetAssignationOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return await AssignationsDBSet.OrderByDescending(a => a.AssignationDate).FirstOrDefaultAsync(a => a.AssignationDate <= date && a.ObjectIdentifier.Equals(objectIDToFindPerVal), token);
        } 

        public async Task<TAssignationValueType?> GetAssignationValueOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return (await GetAssignationOnDate(date, objectIDToFindPerVal, token))?.Value ?? null;
        } 

        public virtual async Task AddAssignationAsync(TAssignationType assignation, CancellationToken token = default)
        {
            if (! await DbSet.AnyAsync(e => e.ID.Equals(assignation.ObjectIdentifier), token))
            {
                throw new ObjectNotFoundException($"{assignation.GetType().Name} with ID = {assignation.ObjectIdentifier} not found");
            }

            assignation.AssignationDate = new DateTime(assignation.AssignationDate.Year, assignation.AssignationDate.Month, 1, 0, 0, 0);
            await AssignationsDBSet.AddAsync(assignation, token);
            await SaveChangesAsync(token);
        }

        public virtual async Task EditAssignationAsync(TAssignationIDType id, DateTime assignationActiveDate, TAssignationValueType newValue, DateTime? newAssignationDate, CancellationToken token = default)
        {
            var assignation = await GetAssignationOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignationType).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
            assignation.Value = newValue;
            if (newAssignationDate.HasValue)
            {
                assignation.AssignationDate = new DateTime(newAssignationDate.Value.Year, newAssignationDate.Value.Month, 1, 0, 0, 0);
            }

            await SaveChangesAsync(token);
        }

        public virtual async Task RemoveAssignationAsync(TAssignationIDType id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            var assignation = await GetAssignationOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(TAssignationType).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
            AssignationsDBSet.Remove(assignation);
            await SaveChangesAsync(token);
        }

        public virtual async Task<IEnumerable<TAssignationType>> GetAllAssignationsAsync(CancellationToken token = default)
        {
            return await AssignationsDBSet.ToListAsync(token);
        }
    }
}