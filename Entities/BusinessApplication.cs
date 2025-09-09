namespace AppointmentSystem.Entities
{
    public enum ApplicationStatus
    {
        Bekliyor = 0,
        Tamamlandı = 1,
        İptal = 2
    }
    public class BusinessApplication : BaseEntity
    {
        public string? BusinessName { get; set; } 
        public string? Address { get; set; } 
        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public int UsersId { get; set; }
        public Users? Users { get; set; }
        public ApplicationStatus Status { get; set; }

    }

}