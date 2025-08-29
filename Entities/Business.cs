namespace AppointmentSystem.Entities
{
    public class Business : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public string? Descrption { get; set; } 
         
        
        public ICollection<Appointments>? Appointments { get; set; }
    }
}