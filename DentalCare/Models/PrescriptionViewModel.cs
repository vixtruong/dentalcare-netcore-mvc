namespace DentalCare.Models
{
    public class PrescriptionViewModel
    {
        public string Id { get; set; }

        public string MedicalExamId  { get; set; }

        public DateTime Date { get; set; }

        public List<PrescriptionDetailViewModel> Details { get; set; } = new List<PrescriptionDetailViewModel>();
    }
}
