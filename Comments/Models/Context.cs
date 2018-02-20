using Microsoft.EntityFrameworkCore;

namespace Comments.Models
{
    public partial class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions options) : base(options)
        {
        }
    }
}
