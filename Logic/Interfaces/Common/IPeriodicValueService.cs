using Database.Interfaces;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces.Common
{
    public interface IPeriodicValueService<TEntity, TAssignmentType, TAssignationIDType, TAssignmentValueType, TTypeBeingAssigned> : IDbAccessServise<TEntity>
        where TEntity : class, IPeriodicValueObject<TAssignmentType, TAssignationIDType, TAssignmentValueType, TTypeBeingAssigned>
        where TAssignmentType : class, IPeriodicValueAssignment<TAssignmentValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignmentValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        internal DbSet<TAssignmentType> AssignmentsDBSet { get; }

        public async Task<IEnumerable<TAssignmentType>> GetAssignmentsForObject(TAssignationIDType objectID, CancellationToken token = default)
        {
            return await AssignmentsDBSet.Where(a => a.ObjectIdentifier.Equals(objectID)).ToListAsync(token);
        }

        public async Task<TAssignmentType?> GetAssignmentOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return await AssignmentsDBSet.OrderByDescending(a => a.AssignmentDate).FirstOrDefaultAsync(a => a.AssignmentDate <= date && a.ObjectIdentifier.Equals(objectIDToFindPerVal), token);
        }

        public async Task<TAssignmentValueType?> GetAssignmentValueOnDate(DateTime date, TAssignationIDType objectIDToFindPerVal, CancellationToken token = default)
        {
            return (await GetAssignmentOnDate(date, objectIDToFindPerVal, token))?.Value ?? null;
        }



        public async Task<IEnumerable<(TAssignmentType? value, DateTime date)>> GetAssignmentForEachMonthOnPeriodAsync(TAssignationIDType objectID, DateTime dateStart, DateTime dateEnd, CancellationToken token = default)
        {//тут стоит переделать в алгоритмы, как минимум, которые не будут каждый раз обращаться в БД.
            (dateStart, dateEnd) = dateStart > dateEnd ? (dateEnd, dateStart) : (dateStart, dateEnd);
            var orderedMonths = DateTimeProvider.GetDateRangeViaAddMonth(dateStart, dateEnd).OrderBy(d=>d);
            //return orderedMonths.Select(m => (AssignmentsDBSet.OrderByDescending(a => a.AssignmentDate).FirstOrDefault(a => a.ObjectIdentifier.Equals(objectID) && a.AssignmentDate <= m), m ));

            var firstDateAssignment = await GetAssignmentOnDate(orderedMonths.First(), objectID, token);
            var lastDateAssignment = await GetAssignmentOnDate(orderedMonths.Last(), objectID, token);
            IEnumerable<TAssignmentType> assignments;
            if (firstDateAssignment == null && lastDateAssignment == null)
            {
                return orderedMonths.Select(m =>
                {
                    TAssignmentType? value = null;
                    return (value, m);
                });
            }
            else if (lastDateAssignment != null && firstDateAssignment != null)
            {
                assignments = await AssignmentsDBSet.Where(a => a.AssignmentDate >= firstDateAssignment.AssignmentDate && a.AssignmentDate <= lastDateAssignment.AssignmentDate).ToListAsync();
            }
            else if (lastDateAssignment != null && firstDateAssignment == null)
            {
                assignments = await AssignmentsDBSet.Where(a => a.AssignmentDate <= lastDateAssignment.AssignmentDate).ToListAsync();
            }
            else
            {
                throw new InvalidOperationException("Logic exception. This mustn't happen");
            }

            assignments = assignments.OrderByDescending(a => a.AssignmentDate);
            return orderedMonths.Select(m => (assignments.FirstOrDefault(a => a.ObjectIdentifier.Equals(objectID) && a.AssignmentDate <= m), m ));
        }
    }
}