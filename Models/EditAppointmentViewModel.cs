using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class EditAppointmentViewModel
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string? UserName { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string? AppointmentNote { get; set; }
        public int BusinessId { get; set; }              
        public List<SelectListItem>? Businesses { get; set; } 

        public AppointmentStatus Status { get; set; }   
    }
}
