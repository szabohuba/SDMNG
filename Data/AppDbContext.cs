using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SDMNG.Models;

namespace SDMNG.Data
{
    public class AppDbContext : IdentityDbContext<Contact>  // Make sure Contact inherits from IdentityUser
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets for your entities
        public DbSet<Contact> Contacts { get; set; }  // Added DbSet for Contact
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TransportRoute> TransportRoutes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<AdminMessage> AdminMessages { get; set; }
        public DbSet<AdminTask> AdminTasks { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tickets-Schedules relationship
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Schedule)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Tickets-Contact relationship (one-to-many)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Contact)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // RouteStop configuration
            modelBuilder.Entity<RouteStop>()
                .HasKey(rs => rs.RouteStopId); // Make sure primary key is defined here

            // Additional configuration (your previous setup)
            modelBuilder.Entity<RouteStop>()
                .HasIndex(rs => new { rs.TransportRouteId, rs.SequenceNumber })
                .IsUnique();

            modelBuilder.Entity<RouteStop>()
                .HasIndex(rs => new { rs.TransportRouteId, rs.StopId })
                .IsUnique();

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.TransportRoute)
                .WithMany(r => r.RouteStop)
                .HasForeignKey(rs => rs.TransportRouteId);

            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.Stop)
                .WithMany(s => s.RouteStop)
                .HasForeignKey(rs => rs.StopId);


        }
    }
}
