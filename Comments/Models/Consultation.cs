using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
    public class ConsultationsContext : DbContext
    {
        public ConsultationsContext(DbContextOptions<ConsultationsContext> options) : base(options) {}

        public DbSet<Consultation> Consultations { get; set; }
    }

    public class Consultation
    {
        public int ConsultationId { get; set; }
        public string GuidanceReference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }

        public Consultation() {}

        public Consultation(string guidanceReference, DateTime startDate, DateTime endDate, string title)
        {
            GuidanceReference = guidanceReference;
            StartDate = startDate;
            EndDate = endDate;
            Title = title;
        }
    }
}
