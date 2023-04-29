﻿using Database.Entities;
using Logic.Models.MonthReports;

namespace Logic.Interfaces
{
    public interface IContractService
    {
        public Task<IEnumerable<Contract>> GetUserContracts(string UserName);
        public Task Add(Contract contract);
        public Task Edit(Contract contract);
        public Task Delete(int id);
        public Task Delete(Contract contract);
        public Task<IEnumerable<Contract>> GetAll();
        public Task ConfirmContractAsync(int contractID, string adminLogin);
        public Task<Contract?> GetContractAsync(int id);
        public Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID);
        public Task UpdateMonthReport(MonthReport monthReport);
        public Task<List<MonthReport>> GetMonthReportAsyncOnDate(DateTime date);
        public Task<string?> GetOwnersLoginAsync(int contractID);
        public Task<MonthReportsUntakenTimeModel> GetMonthReportsUntakenTime(int contractID, IEnumerable<(int contractID,int year,int month)> exceptValuesWithKeys);
    }
}
