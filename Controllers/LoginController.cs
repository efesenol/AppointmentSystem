using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppointmentSystem.Models;
using AppointmentSystem.Data;
using AppointmentSystem.Entities;
using System.Data;


public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    public readonly MyContext _context;

    public LoginController(ILogger<LoginController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;

    }
    [HttpGet]
    [Route("login")]
    public IActionResult Login()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId != null) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [Route("Loginnow")]
    public JsonResult Logina([FromForm] string email, [FromForm] string password)
    {
        var viewModel = new LoginViewModel();

        try
        {
            if (_context == null)
            {
                throw new Exception("Database context (_context) null geliyor!");
            }

            if (_context.Users == null)
            {
                throw new Exception("Users tablosu (_context.Users) null geliyor!");
            }

            var Users = _context.Users
                .Where(a => a.IsActive == true && a.Email == email && a.Password == password)
                .FirstOrDefault();

            if (Users != null)
            {
                HttpContext.Session.SetInt32("Usersid", Users.Id);
                viewModel.ResultID = 1;
                string fullName = Users.FullName!;
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    fullName = char.ToUpper(fullName[0]) + fullName.Substring(1).ToLower();
                }
                viewModel.ResultMessage = $"Hoş geldiniz {fullName}";

                viewModel.RedirectURL = "/";

                return Json(viewModel);
            }

            viewModel.ResultID = -1;
            viewModel.ResultMessage = "E-mail veya şifre hatalı!";
            viewModel.RedirectURL = "#";
        }
        catch (Exception ex)
        {
            viewModel.ResultID = -2;
            viewModel.ResultMessage = "Bir hata oluştu: " + ex.Message;
            viewModel.RedirectURL = "#";
        }

        return Json(viewModel);
    }


    [Route("profilim")]
public IActionResult Profile()
{
    var userId = HttpContext.Session.GetInt32("Usersid");
    if (userId == null) return RedirectToAction("Login", "Login");

    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
    if (user == null) return NotFound();

    // Kullanıcıya ait randevular
    var userAppointments = _context.Appointments
        .Where(a => a.UsersId == userId)
        .ToList();

    var vm = new ProfileViewModel
    {
        Id = user.Id,
        UserName = user.FullName,
        Phone = user.Phone,
        Email = user.Email,
        CreateTime = user.CreateTime,

        // Sayılar
        TotalAppointments = userAppointments.Count,
        Completed = userAppointments.Count(a => a.Status == AppointmentStatus.Tamamlandı),
        Pending = userAppointments.Count(a => a.Status == AppointmentStatus.Bekliyor),
        Cancelled = userAppointments.Count(a => a.Status == AppointmentStatus.İptal)
    };

    return View(vm);
}



    [HttpPost]
[Route("profilim")]
public IActionResult Profile(ProfileViewModel model)
{
    var userId = HttpContext.Session.GetInt32("Usersid");
    if (userId == null) return RedirectToAction("Login", "Login");

    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
    if (user == null) return NotFound();

    bool changed = false;

    // Telefon / Email güncelleme
    if (user.Phone != model.Phone)
    {
        user.Phone = model.Phone;
        changed = true;
    }
    if (user.Email != model.Email)
    {
        user.Email = model.Email;
        changed = true;
    }

    // Şifre güncelleme
    if (!string.IsNullOrEmpty(model.CurrentPassword) ||
        !string.IsNullOrEmpty(model.NewPassword) ||
        !string.IsNullOrEmpty(model.ConfirmPassword))
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            TempData["ErrorMessage"] = "Yeni şifre ile şifre tekrarı aynı değil!";
            return RedirectToAction("Profile");
        }

        if (user.Password != model.CurrentPassword)
        {
            TempData["ErrorMessage"] = "Mevcut şifrenizi yanlış girdiniz!";
            return RedirectToAction("Profile");
        }

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            user.Password = model.NewPassword; // burada hash kullanman gerekir!
            changed = true;
        }
    }

    if (!changed)
    {
        TempData["WarningMessage"] = "Hiçbir değişiklik yapılmadı.";
        return RedirectToAction("Profile");
    }

    user.CreateTime = DateTime.Now;
    _context.SaveChanges();

    TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
    return RedirectToAction("Profile");
}




}