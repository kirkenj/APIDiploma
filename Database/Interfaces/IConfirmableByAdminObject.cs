using Database.Entities;

namespace Database.Interfaces
{
    public interface IConfirmableByAdminObject
    {
        public int ID { get; set; }
        public bool IsConfirmed { get; }
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}
