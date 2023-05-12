namespace Database.Interfaces
{
    public interface IIdObject<T> where T : struct
    {
        public T ID { get; set; }
    }
}
