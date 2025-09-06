using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class AppointmentBusinessViewModel
    {
        public int AppointmentId { get; set; }
        public string? CustomerName { get; set; }  
        public DateTime AppointmentDate { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; } 
        public string? BusinessName { get; set; }
    }
}
