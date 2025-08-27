namespace AppointmentSystem.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
    }
    
}