namespace DentalCare.Models
{
    public class RecoverPasswordViewModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Otp { get; set; }
    }
}
