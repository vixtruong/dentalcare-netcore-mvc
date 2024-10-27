using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DentalCare.Controllers
{
    public class AccountController : Controller
    {
        private AccountService _accountService;
        private DoctorService _doctorService;
        private ReceptionistService _receptionistService;
        private FacultyService _facultyService;

        public AccountController(AccountService accountService, DoctorService doctorService, ReceptionistService receptionistService, FacultyService facultyService)
        {
            _accountService = accountService;
            _doctorService = doctorService;
            _receptionistService = receptionistService;
            _facultyService = facultyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Account account)
        {
            var allAccounts = _accountService.GetAll();
            var existingAccount = allAccounts.FirstOrDefault(a => a.Phone == account.Phone && a.Password == account.Password);

            if (existingAccount != null)
            {
                bool isDoctor = existingAccount.DoctorId != null;
                string userId;
                string userName;
                string userAvatar;

                if (isDoctor)
                {
                    var doctor = _doctorService.Get(existingAccount.DoctorId);
                    if (doctor.Fired == true)
                    {
                        ViewBag.Error = true;
                        ViewBag.ErrorMessage = "You have been terminated.";
                        return View();
                    }
                    userId = existingAccount.DoctorId;
                    userName = doctor.Name;
                    userAvatar = doctor.Avatar;
                }
                else
                {
                    var receptionist = _receptionistService.Get(existingAccount.ReceptionistId);
                    if (receptionist.Fired == true)
                    {
                        ViewBag.Error = true;
                        ViewBag.ErrorMessage = "You have been terminated.";
                        return View();
                    }
                    userId = existingAccount.ReceptionistId;
                    userName = receptionist.Name;
                    userAvatar = receptionist.Avatar;
                }

                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserName", userName);
                HttpContext.Session.SetString("UserAvatar", userAvatar);
                HttpContext.Session.SetString("UserRole", existingAccount.Role);

                var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, existingAccount.Phone),
                                        new Claim(ClaimTypes.Role, existingAccount.Role)
                                    };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = "Incorrect login information. Please try again.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var faculties = _facultyService.GetAll();
            ViewBag.Faculties = faculties;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            try
            {
                if (model.Avatar != null && model.Avatar.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                    var fileName = Path.GetFileName(model.Avatar.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(fileStream);
                    }

                    model.AvatarPath = $"/uploads/avatars/{fileName}";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "File upload failed");
            }


            if (userId.Contains("D"))
            {
                var doctor = _doctorService.Get(userId);

                if (doctor.Phone != model.Phone)
                {
                    var account = _accountService.GetByDoctorId(doctor.Id);

                    var newAccount = new Account
                    {
                        DoctorId = account.DoctorId,
                        Phone = model.Phone,
                        Email = model.Email,
                        Password = account.Password,
                        Role = account.Role,
                    };

                    _accountService.Delete(account.Phone);
                    _accountService.Add(newAccount);
                }

                doctor.Facultyid = model.Faculty;
                doctor.FacultyName = _facultyService.Get(model.Faculty).Name;
                doctor.Phone = model.Phone;
                doctor.Email = model.Email;
                doctor.Gender = model.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase);
                doctor.Name = model.FullName;
                doctor.Birthday = model.Birthday.Value;
                doctor.Firstdayofwork = model.JoinTime.Value;
                doctor.Avatar = (model.AvatarPath != null) ? model.AvatarPath : doctor.Avatar;

                _doctorService.Update(doctor);
            }
            else
            {
                var receptionist = _receptionistService.Get(userId);

                if (receptionist.Phone != model.Phone)
                {
                    var account = _accountService.GetByReceptionistId(receptionist.Id);

                    var newAccount = new Account
                    {
                        ReceptionistId = account.ReceptionistId,
                        Phone = model.Phone,
                        Email = model.Email,
                        Password = account.Password,
                        Role = account.Role,
                    };

                    _accountService.Delete(account.Phone);
                    _accountService.Add(newAccount);
                }

                receptionist.Name = model.FullName;
                receptionist.Phone = model.Phone;
                receptionist.Email = model.Email;
                receptionist.Gender = model.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase);
                receptionist.Birthday = model.Birthday.Value;
                receptionist.Firstdayofwork = model.JoinTime.Value;
                receptionist.Avatar = (model.AvatarPath != null) ? model.AvatarPath : receptionist.Avatar;

                _receptionistService.Update(receptionist);
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var phone = HttpContext.Session.GetString("Phone");
            var storedPassword = _accountService.Get(phone)?.Password; // Safe navigation to avoid null reference
            var storedOtp = HttpContext.Session.GetString("OTP");

            if (storedPassword == null || model.OldPassword != storedPassword)
            {
                ViewBag.Error = true;
                ViewBag.Message = "Old password is incorrect.";
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.Error = true;
                ViewBag.Message = "New password and confirmation password do not match.";
                return View(model);
            }

            if (model.Otp != storedOtp)
            {
                ViewBag.Error = true;
                ViewBag.Message = "Invalid OTP.";
                return View(model);
            }

            var account = _accountService.Get(phone);
            if (account != null)
            {
                account.Password = model.NewPassword;
                _accountService.Update(account);
            }

            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("Password");

            ViewBag.Error = false;
            ViewBag.Message = "Password changed successfully!";
            return View();
        }

        [HttpGet]
        public IActionResult Recover()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Recover(RecoverPasswordViewModel model)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");

            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.Error = true;
                ViewBag.Message = "New password and confirmation password do not match.";
                return View(model);
            }

            if (model.Otp != storedOtp)
            {
                ViewBag.Error = true;
                ViewBag.Message = "Invalid OTP.";
                return View(model);
            }

            var account = _accountService.Get(model.Email);
            account.Password = model.NewPassword;

            _accountService.Update(account);

            HttpContext.Session.Remove("OTP");

            ViewBag.Error = false;
            ViewBag.Message = "Password changed successfully!";
            return View();
        }

        public IActionResult SendOTPToChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            string userEmail;

            // Get user email based on user role
            if (userId.Contains("D"))
            {
                userEmail = _doctorService.Get(userId).Email;
            }
            else
            {
                userEmail = _receptionistService.Get(userId).Email;
            }

            // Generate OTP
            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();

            // Store OTP in session
            HttpContext.Session.SetString("OTP", otp);

            // Prepare email content
            string from = "n8cnpm2024@gmail.com";
            string pass = "ymhp dxyc jgti ohne";
            string to = userEmail;
            string messageBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; overflow: hidden;'>
                                <div style='background-color: #0066cc; color: white; padding: 20px; text-align: center;'>
                                    <h1>DentalCare</h1>
                                    <h1>OTP to Change Password</h1>
                                </div>
                                <div style='padding: 20px; text-align: center;'>
                                    <h2 style='color: #0066cc;'>{otp}</h2>
                                </div>
                            </div>
                        </body>
                        </html>";

            // Create and send the email
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = messageBody;
            message.Subject = "OTP Change Password - DentalCare";
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
                return PartialView("_OtpMessage", new { success = true, message = "OTP sent successfully. Please check your email." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return PartialView("_OtpMessage", new { success = false, message = "Failed to send OTP. Please try again." });
            }
        }

        public IActionResult SendOTPToResetPassword(string email)
        {
            var account = _accountService.Get(email);
            if (account == null)
            {
                return PartialView("_OtpMessage", new { success = false, message = "Email is not exist." });
            }

            // Generate OTP
            Random rand = new Random();
            string otp = rand.Next(100000, 999999).ToString();

            // Store OTP in session
            HttpContext.Session.SetString("OTP", otp);

            // Prepare email content
            string from = "n8cnpm2024@gmail.com";
            string pass = "ymhp dxyc jgti ohne";
            string to = email;
            string messageBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                            <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; overflow: hidden;'>
                                <div style='background-color: #0066cc; color: white; padding: 20px; text-align: center;'>
                                    <h1>DentalCare</h1>
                                    <h1>OTP to Change Password</h1>
                                </div>
                                <div style='padding: 20px; text-align: center;'>
                                    <h2 style='color: #0066cc;'>{otp}</h2>
                                </div>
                            </div>
                        </body>
                        </html>";

            // Create and send the email
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.From = new MailAddress(from);
            message.Body = messageBody;
            message.Subject = "OTP Change Password - DentalCare";
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
                return PartialView("_OtpMessage", new { success = true, message = "OTP sent successfully. Please check your email." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return PartialView("_OtpMessage", new { success = false, message = "Failed to send OTP. Please try again." });
            }
        }
    }
}
