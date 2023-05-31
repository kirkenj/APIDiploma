namespace WebFront.Models.Account
{
    public class UserViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string NSP => string.Join(' ', Name, Surname, Patronymic);
        public int RoleId { get; set; }
        public string Login { get; set; } = null!;
    }
}
