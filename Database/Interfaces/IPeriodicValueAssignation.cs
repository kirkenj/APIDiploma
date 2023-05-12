namespace Database.Interfaces
{
    public interface IPeriodicValueAssignation<TValueType, TObjectIDType, TEntity>
        where TObjectIDType : struct 
        where TValueType : struct
        where TEntity : IIdObject<TObjectIDType> 
    { 
        public DateTime AssignationDate { get; set; }
        public TValueType Value { get; set; }
        public TObjectIDType ObjectIdentifier { get; set; }
        public TEntity ObjectRef { get; set; }
    }
}
