using Database.Entities;

namespace Database.Interfaces
{
    public interface IConfirmableByAdminObjectAbstract
    {
        public bool IsConfirmed { get; }
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}
