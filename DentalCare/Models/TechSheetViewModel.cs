namespace DentalCare.Models
{
    public class TechSheetViewModel
    {
        public string Id { get; set; }

        public string MedicalExamId { get; set; }

        public DateTime Date { get; set; }

        public List<TechDetailViewModel> Details { get; set; } = new List<TechDetailViewModel>();
    }
}
