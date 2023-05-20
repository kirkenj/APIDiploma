namespace Logic.Models.User
{
    public class UserSelectObject
    {
        public IEnumerable<int>? IDs { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Login { get; set; }
        public IEnumerable<int>? RoleIds { get; set; }
    }
}