using Database.Entities;

namespace Logic.Interfaces
{
    public interface IContractTypeService : IDbAccessServise<ContractType>, IPeriodicValueService<ContractType, ContractTypePriceAssignment, int, double, ContractType>
    {
    }
}
