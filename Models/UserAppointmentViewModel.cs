using AppointmentSystem.Entities;

namespace AppointmentSystem.Models
{
    public class UserAppointmentViewModel
    {

        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int BusinessId { get; set; }
        public string? BusinessName { get; set; }

        public DateTime AppointmentDate { get; set; }

        public AppointmentStatus AppointmentStatus { get; set; }
        public string StatusText => AppointmentStatus.ToString();
    }
}
