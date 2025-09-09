using AppointmentSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<BusinessApplication> BusinessApplication { get; set; }
        
       
    }

}