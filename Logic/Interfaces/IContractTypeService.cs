using Database.Entities;
using Logic.Interfaces.Common;

namespace Logic.Interfaces
{
    public interface IContractTypeService : IDbAccessServise<ContractType>, IPeriodicValueServiceWithEdit<ContractType, ContractTypePriceAssignment, int, double, ContractType>
    {
    }
}
