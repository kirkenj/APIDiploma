namespace Database.Interfaces
{
    public interface IPeriodicValueObject<TAssignationType, TAssignationIDType, TAssignationValueType, TTypeBeingAssigned> : IIdObject<TAssignationIDType> 
        where TAssignationType : IPeriodicValueAssignation<TAssignationValueType, TAssignationIDType, TTypeBeingAssigned>
        where TAssignationIDType : struct
        where TAssignationValueType : struct
        where TTypeBeingAssigned : IIdObject<TAssignationIDType>
    {
        public IEnumerable<TAssignationType> Assignations { get; set; }
    }
}
