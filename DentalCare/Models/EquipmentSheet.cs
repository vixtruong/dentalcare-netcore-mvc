namespace DentalCare.Models
{
    public partial class EquipmentSheet
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string MedicalexaminationId { get; set; } = null!;

        public virtual Medicalexamination Medicalexamination { get; set; }

        public virtual ICollection<Equipmentdetail> Equipmentdetails { get; set; } = new List<Equipmentdetail>();
    }
}
