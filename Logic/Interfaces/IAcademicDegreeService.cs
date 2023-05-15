using Database.Entities;
using Logic.Interfaces.Common;

namespace Logic.Interfaces
{
    public interface IAcademicDegreeService : IDbAccessServise<AcademicDegree>, IPeriodicValueServiceWithEdit<AcademicDegree, AcademicDegreePriceAssignation, int, double, AcademicDegree>
    {
    }
}
    