using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.ContractType;
using Logic.Models.Department;

namespace Logic.Interfaces
{
    public interface IDepartmentService : IDbAccessServise<Department>, IGetViaSelectObjectService<Department, DepartmentSelectObject>
    {
    }
}
