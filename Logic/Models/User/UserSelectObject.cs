namespace Logic.Models.User
{
    public class UserSelectObject
    {
        public IEnumerable<int>? IDs { get; set; }
        public string? NSP { get; set; }
        public string? Login { get; set; }
        public IEnumerable<int>? RoleIds { get; set; }
    }
}