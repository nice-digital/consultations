using Microsoft.EntityFrameworkCore;

namespace Comments.Models
{
    public class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions<ConsultationsContext> options) : base(options) {}

        public DbSet<Consultation> Consultations { get; set; }
    }
}