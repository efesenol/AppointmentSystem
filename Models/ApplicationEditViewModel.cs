using AppointmentSystem.Entities;

namespace AppointmentSystem.Models
{
    public class ApplicationEditViewModel
    {
        public int ApplicationId { get; set; }
        public string? BusinessName { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
         public IFormFile? ImgFile { get; set; }
        public string? ImgUrl { get; set; }
        public ApplicationStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public int UsersId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        


    }
}