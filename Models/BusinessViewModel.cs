
using AppointmentSystem.Entities;

namespace AppointmentSystem.Models
{
	public class BusinessViewModel 
	{
        public int BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessDescrption { get; set; }
        public string? BusinessAdress { get; set; }
    }
}
