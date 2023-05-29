namespace WebFront.RequestModels.Account
{
    public class UserViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public int RoleId { get; set; }
        public string Login { get; set; } = null!;
    }
}
