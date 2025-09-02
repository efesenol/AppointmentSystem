using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreateTime { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

        public int TotalAppointments { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }


    }
}

