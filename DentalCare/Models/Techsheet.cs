namespace DentalCare.Models
{
    public partial class Techsheet
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string MedicalexaminationId { get; set; } = null!;

        public virtual Medicalexamination Medicalexamination { get; set; }

        public virtual ICollection<Techdetail> Techdetails { get; set; } = new List<Techdetail>();

    }
}
