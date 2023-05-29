using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.AcademicDegree;
using Logic.Models.User;

namespace Logic.Interfaces
{
    public interface IAcademicDegreeService : IDbAccessServise<AcademicDegree>, IPeriodicValueServiceWithEdit<AcademicDegree, AcademicDegreePriceAssignment, int, double, AcademicDegree>
        , IGetViaSelectObjectService<AcademicDegree, AcademicDegreeSelectObject>
    {
    }
}
    