using Database.Entities;

namespace Logic.Interfaces
{
    public interface IAcademicDegreeService : IDbAccessServise<AcademicDegree>, IPeriodicValueService<AcademicDegree, AcademicDegreePriceAssignation, int, double, AcademicDegree>
    {
    }
}
    