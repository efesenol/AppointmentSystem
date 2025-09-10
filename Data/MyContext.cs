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


   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Appointments>()
            .HasOne(a => a.Users)
            .WithMany()
            .HasForeignKey(a => a.UsersId)
            .OnDelete(DeleteBehavior.Restrict);

       
        modelBuilder.Entity<Appointments>()
            .HasOne(a => a.Business)
            .WithMany()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);
    }
 }
}