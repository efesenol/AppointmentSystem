
using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class AppointmentDetailViewModel
    {
        public int BusinessId { get; set; }
        public int AppointmentId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string? Note { get; set; }
        public AppointmentStatus Status { get; set; } 
    }
}
