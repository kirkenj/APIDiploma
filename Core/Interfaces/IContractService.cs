using Database.Entities;

namespace Logic.Interfaces
{
    public interface IContractService
    {
        public Task<IEnumerable<Contract>> GetUserContracts(string UserName);
        public Task Add(Contract contract);
        public Task<IEnumerable<Contract>> GetAll(bool doIncludeAdditionalInfo = true);
        public Task ConfirmContract(Contract contract, User user);
        public Task<Contract?> GetById(int id);
        public Task UpdateMonthReport(MonthReport monthReport);
    }
}
