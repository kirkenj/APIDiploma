namespace Database.Interfaces
{
    public interface IPeriodicValueObject<TAssignmentType, TAssignmentDType, TAssignmentValueType, TTypeBeingAssigned> : IIdObject<TAssignmentDType> 
        where TAssignmentType : IPeriodicValueAssignment<TAssignmentValueType, TAssignmentDType, TTypeBeingAssigned>
        where TAssignmentDType : struct
        where TAssignmentValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignmentDType>
    {
        protected internal IEnumerable<TAssignmentType> Assignments { get; set; }
    }
}
