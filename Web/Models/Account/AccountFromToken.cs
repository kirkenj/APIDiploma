using Web.RequestModels.Account;

namespace Web.Models.Account
{
    public class AccountFromToken
    {
        public int ID { get; set; }
        public string Role { get; set; } = null!;
        public string Login { get; set; } = null!;
        public UserViewModel? UserAccountInDB { get; set; }
    }
}
