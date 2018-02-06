using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;

namespace Comments.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly ConsultationsContext _consultationsContext;

        public ConsultationService(ConsultationsContext consultationsContext)
        {
            _consultationsContext = consultationsContext;
        }

        public List<Consultation> GetAllConsultations()
        {
            return _consultationsContext.Consultations.ToList();
        }
    }

    public interface IConsultationService
    {
        List<Consultation> GetAllConsultations();
    }
}
