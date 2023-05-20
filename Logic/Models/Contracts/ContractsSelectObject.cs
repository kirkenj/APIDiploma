namespace Logic.Models.Contracts
{
    public class ContractsSelectObject
    {
        public IEnumerable<int>? ContractIDs { get; set; } = null!;
        public IEnumerable<int>? UserIDs { get; set; } = null!;
        public IEnumerable<int>? DepartmentIDs { get; set; }
        public IEnumerable<int>? ContractTypeIDs { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime? PeriodStartStartBound { get; set; }
        public DateTime? PeriodStartEndBound { get; set; }
        public DateTime? PeriodEndStartBound { get; set; }
        public DateTime? PeriodEndEndBound { get; set; }
        public double? TimeSumStartBound { get; set; }
        public double? TimeSumEndBound { get; set; }
    }
}
