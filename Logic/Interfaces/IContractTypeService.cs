using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.ContractType;

namespace Logic.Interfaces
{
    public interface IContractTypeService : IDbAccessServise<ContractType>, IPeriodicValueServiceWithEdit<ContractType, ContractTypePriceAssignment, int, double, ContractType>, IGetViaSelectObjectService<ContractType, ContractTypesSelectObject>
    {
    }
}
