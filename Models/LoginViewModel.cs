
using AppointmentSystem.Entities;

namespace AppointmentSystem.Models
{
    public class LoginViewModel : BaseResult
    {
        public Users? User { get; set; }

    }
    
    public class RegisterViewModel : BaseEntity
	{
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public Users? User { get; set; }
       
    }
}
