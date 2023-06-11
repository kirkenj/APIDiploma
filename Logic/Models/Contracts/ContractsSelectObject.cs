namespace Logic.Models.Contracts
{
    public class ContractsSelectObject
    {
        public IEnumerable<int>? UserIDs { get; set; } = null!;
        public IEnumerable<int>? DepartmentIDs { get; set; }
        public IEnumerable<int>? ContractTypeIDs { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime? ConclusionDateStartBound { get; set; }
        public DateTime? ConclusionDateEndBound { get; set; }
        public DateTime? PeriodStartStartBound { get; set; }
        public DateTime? PeriodStartEndBound { get; set; }
        public DateTime? PeriodEndStartBound { get; set; }
        public DateTime? PeriodEndEndBound { get; set; }
        public string? IdentifierPart { get; set; }
    }
}
