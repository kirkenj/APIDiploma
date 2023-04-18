namespace Database.Entities
{
    public class Department
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<Contract> Contracts { get; set; } = null!;
    }
}
