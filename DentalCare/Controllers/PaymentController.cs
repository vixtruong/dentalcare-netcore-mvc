using DentalCare.Models;
using DentalCare.Payments.Momo.Config;
using DentalCare.Payments.Momo.Request;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DentalCare.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly MomoConfig _momoConfig;
        private readonly InvoiceService _invoiceService;

        public PaymentController(IOptions<MomoConfig> momoConfig, InvoiceService invoiceService)
        {
            _momoConfig = momoConfig.Value;
            _invoiceService = invoiceService;
        }

        [HttpGet("momo-return")]
        public IActionResult ReturnFromMomo([FromQuery] MomoOneTimePaymentResultRequest result)
        {
            if (result.IsValidSignature(_momoConfig.AccessKey, _momoConfig.SecretKey))
            {
                if (result.resultCode == 0) // Thanh toán thành công
                {
                    var discount = int.TryParse(result.extraData, out var parsedDiscount) ? parsedDiscount : 0;
                    var bill = new Bill
                    {
                        Id = result.orderId.Substring(0, Math.Min(10, result.orderId.Length)), // Giả sử orderId là mã hóa đơn
                        Medicalexaminationid = result.orderInfo,
                        Date = DateTime.Today,
                        Total = (int)result.amount * 100 / (100 - discount),
                        Discount = (byte)discount,
                        Finaltotal = (int)result.amount,
                        Payment = "Transfer",
                        Receptionistid = HttpContext.Session.GetString("UserId")
                    };

                    _invoiceService.Add(bill);
                    TempData["SuccessMessage"] = "Payment success. Invoice has been created.";
                    return RedirectToAction("Index", "Invoice");
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment failed!";
                    return RedirectToAction("Add", "Invoice");
                }
            }

            TempData["ErrorMessage"] = "Invalid signature.";
            return RedirectToAction("Add", "Invoice");
        }
    }
}
