namespace Database.Interfaces
{
    public interface IPeriodicValueAssignment<TValueType, TObjectIDType, TEntity>
        where TObjectIDType : struct 
        where TValueType : struct
        where TEntity : IIdObject<TObjectIDType> 
    { 
        public DateTime AssignmentDate { get; set; }
        public TValueType Value { get; set; }
        public TObjectIDType ObjectIdentifier { get; set; }
        protected internal TEntity ObjectRef { get; set; }
    }
}
