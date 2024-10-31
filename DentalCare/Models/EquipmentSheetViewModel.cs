namespace DentalCare.Models
{
    public class EquipmentSheetViewModel
    {
        public string Id { get; set; }

        public string MedicalExamId { get; set; }

        public DateTime Date { get; set; }

        public List<EquipmentDetailViewModel> Details { get; set; } = new List<EquipmentDetailViewModel>();
    }
}
