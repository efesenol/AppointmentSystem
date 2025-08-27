namespace AppointmentSystem.Entities
{
    public enum AppointmentStatus
        {
            Bekliyor = 0,
            Tamamlandı = 1,
            İptal = 2
        }
    public class Appointments : BaseEntity
    {
        public DateTime AppointmentDate { get; set; }
       
        public AppointmentStatus Status { get; set; }
        public int UsersId { get; set; }
        public Users? Users { get; set; }
        public int BusinessId { get; set; }
        public Business? Business { get; set; }

    }

}