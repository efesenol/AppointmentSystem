
using AppointmentSystem.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentSystem.Models
{
    public class MyBusinessEditViewModel
    {
        public int BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessTel { get; set; }
        public string? BusinessDescrption { get; set; }
        public string? ImgUrl { get; set; }
        public IFormFile? ImgFile { get; set; }

        

    }
}
