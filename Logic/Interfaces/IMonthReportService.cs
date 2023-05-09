using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces
{
    public interface IMonthReportService : IDbAccessServise<MonthReport>
    {
        internal DbSet<MonthReport> DBSet => DbSet;
    }
}
