using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models.MonthReports
{
    public class RelatedContractsWithReportsObject
    {
        public IEnumerable<Contract> Contracts { get; set; } = null!;
        public IEnumerable<MonthReport> Reports { get; set; } = null!;
    }
}
