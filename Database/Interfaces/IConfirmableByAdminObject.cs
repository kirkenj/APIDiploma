using Database.Entities;

namespace Database.Interfaces
{
    public interface IConfirmableByAdminObject
    {
        public bool IsConfirmed { get; }
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}
