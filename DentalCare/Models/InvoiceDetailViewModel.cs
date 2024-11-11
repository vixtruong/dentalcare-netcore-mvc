namespace DentalCare.Models
{
    public class InvoiceDetailViewModel
    {
        public Bill Invoice { get; set; }
        public Medicalexamination MedicalExam { get; set; }
        public Customer Customer { get; set; }
        public Receptionist Receptionist { get; set; }
        public List<Techdetail> TechDetails { get; set; }
        public List<Techposition> TechWorks { get; set; }
        public List<Prescriptiondetail> PrescriptionDetails { get; set; }
        public List<Medicine> Medicines { get; set; }
    }
}
