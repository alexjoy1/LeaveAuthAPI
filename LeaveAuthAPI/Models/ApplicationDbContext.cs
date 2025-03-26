using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public object LeaveRequests { get; internal set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Store hashed passwords in a real-world app!
    }
    public class LeaveRequests
    {
        public int Id { get; set; }
        public string EmployeeUsername { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";
    }

}
