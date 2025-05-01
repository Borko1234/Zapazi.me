using Booking.Data.Entities;
using Booking.Data.Identity.Roles;
using Booking.Data.Identity.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data
{
    public class ApplicationDbContext : IdentityDbContext<BookingUser, BookingRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Schedule> Schedules {  get; set; }
        public DbSet<FacilitySchedule> FacilitySchedules { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FacilitySchedules

            modelBuilder.Entity<FacilitySchedule>()
                .HasKey(fs => new { fs.Id });

            modelBuilder.Entity<FacilitySchedule>()
                .HasIndex(fs => new { fs.FacilityId, fs.ScheduleId })
                .IsUnique();

            modelBuilder.Entity<FacilitySchedule>()
                .HasOne(fs => fs.Facility)
                .WithMany(f => f.FacilitySchedules)
                .HasForeignKey(fs => fs.FacilityId);

            modelBuilder.Entity<FacilitySchedule>()
                .HasOne(fs => fs.Schedule)
                .WithMany(s => s.FacilitySchedules)
                .HasForeignKey(fs => fs.ScheduleId);

            // Reservations
            
            modelBuilder.Entity<Reservation>()
                .HasKey(r => new { r.Id });

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.FacilityId, r.UserId });

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Facility)
                .WithMany(f => f.Reservations)
                .HasForeignKey(r => r.FacilityId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
