using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class EditBusinessViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Telephone { get; set; }
        public string? ImgUrl { get; set; }
         public IFormFile? ImgFile { get; set; }
    }
}
