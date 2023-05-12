using Database.Entities;

namespace Database.Interfaces
{
    public interface IConfirmableByAdminObject : IIdObject<int>
    {
        public bool IsConfirmed { get; }
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
    }
}
