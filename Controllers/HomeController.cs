using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppointmentSystem.Models;
using AppointmentSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppointmentSystem.Entities;
using Microsoft.Identity.Client;

namespace AppointmentSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public readonly MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int id)
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("LandingPage", "Home");

        var appointment = _context.Appointments
        .Include(vm => vm.Users)
        .Include(vm => vm.Business)
        .Where(vm => vm.IsActive == true && vm.IsDelete == false && vm.UsersId == userId)
        .OrderBy(vm => vm.AppointmentDate)
        .Select(vm => new UserAppointmentViewModel
        {
            AppointmentId = vm.Id,
            UserId = vm.Users!.Id,
            UserName = vm.Users.FullName,
            BusinessId = vm.Business!.Id,
            BusinessName = vm.Business.Name,
            BusinessDescrption = vm.Business.Descrption,
            BusinessAdress = vm.Business.Address,
            AppointmentDate = vm.AppointmentDate,
            AppointmentStatus = vm.Status
        })
        .ToList();
        return View(appointment);
    }

    [Route("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        var appointment = _context.Appointments
        .Include(a => a.Users)
        .Include(a => a.Business)
        .FirstOrDefault(a => a.Id == id);

        if (appointment == null) return NotFound();

        var vm = new EditAppointmentViewModel
        {
            AppointmentId = appointment.Id,
            UserName = appointment.Users?.FullName,
            AppointmentDate = appointment.AppointmentDate,
            BusinessId = appointment.BusinessId,
            Status = appointment.Status,
            AppointmentNote = appointment.Note,

            Businesses = _context.Business
            .Where(b => b.IsActive)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name,
            })
            .ToList()
        };
        return View(vm);
    }
    [HttpPost]
    [Route("Edit/{id}")]
    public IActionResult Edit(EditAppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Dropdown boş kalmasın diye tekrar doldur
            model.Businesses = _context.Business
                .Where(b => b.IsActive)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();

            return View(model);
        }
        var appointment = _context.Appointments.Find(model.AppointmentId);
        if (appointment == null) return NotFound();

        appointment.AppointmentDate = model.AppointmentDate;
        appointment.BusinessId = model.BusinessId;
        appointment.Status = model.Status;
        appointment.Note = model.AppointmentNote;

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null) return NotFound();
        appointment.IsDelete = true;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [Route("yeni-randevu")]
    public IActionResult NewAppointment()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null)
            return RedirectToAction("Login", "Login");

        // Kullanıcı bilgisi
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        // Aktif işletmeler dropdown için
        var businesses = _context.Business
            .Where(b => b.IsActive && !b.IsDelete)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            })
            .ToList();

        var vm = new NewAppointmentViewModel
        {
            UserName = user?.FullName,
            AppointmentDate = DateTime.Now, // default şimdi
            Businesses = businesses
        };

        return View(vm);
    }

    [HttpPost]
    [Route("yeni-randevu")]
    public IActionResult NewAppointment(NewAppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // dropdown tekrar doldurulmalı yoksa view boş kalır
            model.Businesses = _context.Business
                .Where(b => b.IsActive && !b.IsDelete)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToList();

            return View(model);
        }

        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        var appointment = new Appointments
        {
            UsersId = userId.Value,
            AppointmentDate = model.AppointmentDate,
            CreateTime = DateTime.Now,
            BusinessId = model.BusinessId,
            Status = AppointmentSystem.Entities.AppointmentStatus.Bekliyor,
            Note = model.AppointmentNote,
            IsActive = true,
            IsDelete = false
        };
        _context.Appointments.Add(appointment);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [Route("isletmeler")]
    public IActionResult Businesses()
    {
        var businesses = _context.Business
        .Where(a => a.IsActive == true && a.IsDelete == false)
        .Select(a => new BusinessViewModel
        {
            BusinessAdress = a.Address,
            BusinessName = a.Name,
            BusinessId = a.Id,
            BusinessDescrption = a.Descrption,
            BusinessImg = a.ImgUrl
        })
        .ToList();
        return View(businesses);
    }
    [Route("randevu-al/{id}")]
    public IActionResult CreateAppointment(int id)
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null)
            return RedirectToAction("Login", "Login");

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        var businesses = _context.Business
            .Where(b => b.IsActive && !b.IsDelete && b.Id == id)
            .FirstOrDefault();

        if (businesses == null) return RedirectToAction("Index");

        var vm = new NewAppointmentViewModel
        {
            UserName = user?.FullName,
            AppointmentDate = DateTime.Now,
            BusinessId = businesses!.Id,
            BusinessName = businesses.Name,
            CreateTime = DateTime.Now,
        };
        return View(vm);
    }


    [HttpPost]
    [Route("randevu-al")]
    public IActionResult CreateAppointment(NewAppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // dropdown tekrar doldurulmalı yoksa view boş kalır
            model.Businesses = _context.Business
                .Where(b => b.IsActive && !b.IsDelete)
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToList();

            return View(model);
        }

        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        var appointment = new Appointments
        {
            UsersId = userId.Value,
            AppointmentDate = model.AppointmentDate,
            BusinessId = model.BusinessId,
            Status = AppointmentSystem.Entities.AppointmentStatus.Bekliyor,
            Note = model.AppointmentNote,
            IsActive = true,
            IsDelete = false
        };

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [Route("/bekleyen-randevular")]
    public IActionResult ActiveAppointment()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        var appointment = _context.Appointments
        .Include(vm => vm.Users)
        .Include(vm => vm.Business)
        .Where(vm => vm.IsActive == true && vm.IsDelete == false && vm.UsersId == userId && vm.AppointmentDate >= DateTime.Now && vm.Status == AppointmentStatus.Bekliyor)
        .Select(vm => new UserAppointmentViewModel
        {
            AppointmentId = vm.Id,
            UserId = vm.Users!.Id,
            UserName = vm.Users.FullName,
            BusinessId = vm.Business!.Id,
            BusinessName = vm.Business.Name,
            BusinessDescrption = vm.Business.Descrption,
            BusinessAdress = vm.Business.Address,
            AppointmentDate = vm.AppointmentDate,
            AppointmentStatus = vm.Status
        })
        .ToList();
        return View(appointment);
    }
    [Route("/gecmis-randevular")]
    public IActionResult OldAppointment()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        var appointment = _context.Appointments
        .Include(vm => vm.Users)
        .Include(vm => vm.Business)
        .Where(vm => vm.IsActive == true && vm.IsDelete == false && vm.UsersId == userId && vm.AppointmentDate <= DateTime.Now)
        .Select(vm => new UserAppointmentViewModel
        {
            AppointmentId = vm.Id,
            UserId = vm.Users!.Id,
            UserName = vm.Users.FullName,
            BusinessId = vm.Business!.Id,
            BusinessName = vm.Business.Name,
            BusinessDescrption = vm.Business.Descrption,
            BusinessAdress = vm.Business.Address,
            AppointmentDate = vm.AppointmentDate,
            AppointmentStatus = vm.Status
        })
        .ToList();
        return View(appointment);
    }

    [Route("tanitim")]
    public IActionResult LandingPage()
    {

        return View();
    }

    [Route("isletmelerim")]
    public IActionResult MyBusiness()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("LandingPage", "Home");

        var businesses = _context.Business
        .Where(a => a.IsActive == true && a.IsDelete == false && userId == a.Users!.Id)
        .Select(a => new BusinessViewModel
        {
            BusinessAdress = a.Address,
            BusinessName = a.Name,
            BusinessId = a.Id,
            BusinessDescrption = a.Descrption,
            BusinessImg = a.ImgUrl
        })
        .ToList();
        return View(businesses);

    }
    [Route("isletme-bilgisi/{id}")]
    public IActionResult MyBusinessDetail(int id)
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("LandingPage", "Home");

        var businesses = _context.Business
        .Where(a => a.IsActive == true && a.IsDelete == false && userId == a.Users!.Id)
        .Select(a => new EditBusinessViewModel
        {
            Address = a.Address,
            Name = a.Name,
            Id = a.Id,
            Description = a.Descrption,
            ImgUrl = a.ImgUrl
        })
        .FirstOrDefault();
        return View(businesses);

    }

    [Route("isletme-randevular/{id}")]
    public IActionResult MyBusinessAppointment(int id)
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("LandingPage", "Home");

        var appointments = _context.Appointments
            .Include(a => a.Users)
            .Include(a => a.Business)
            .Where(a => a.BusinessId == id && a.IsActive && !a.IsDelete)
            .Select(a => new AppointmentBusinessViewModel
            {
                AppointmentId = a.Id,
                CustomerName = a.Users != null ? a.Users.FullName : "Bilinmiyor",
                AppointmentDate = a.AppointmentDate,
                Note = a.Note,
                Status = a.Status.ToString(),
                BusinessName = a.Business != null ? a.Business.Name : ""
            })
            .OrderBy(a => a.AppointmentDate)
            .ToList();

        return View(appointments);
    }
    [Route("randevu-detay/{id}")]
    public IActionResult AppointmentDetail(int id)
    {
        var appointment = _context.Appointments
            .Include(a => a.Users)
            .Include(a => a.Business)
            .Where(a => a.Id == id && a.IsActive && !a.IsDelete)
            .Select(a => new AppointmentDetailViewModel
            {
                BusinessId = a.Business!.Id,
                AppointmentId = a.Id,
                CustomerName = a.Users != null ? a.Users.FullName : "Bilinmiyor",
                CustomerEmail = a.Users != null ? a.Users.Email : "",
                CustomerPhone = a.Users != null ? a.Users.Phone : "",
                BusinessName = a.Business != null ? a.Business.Name : "",
                BusinessAddress = a.Business != null ? a.Business.Address : "",
                AppointmentDate = a.AppointmentDate,
                Note = a.Note,
                Status = a.Status
            })
            .FirstOrDefault();

        if (appointment == null)
            return NotFound();

        return View(appointment);
    }

    [Route("yeni-basvuru")]
    public IActionResult ApplicationBusiness()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("LandingPage", "Home");


        return View();
    }

    [HttpPost]
    [Route("yeni-basvuru")]
    public IActionResult ApplicationBusiness(BusinessApplicationViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? filePath = null;
        if (model.ImgFile != null)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(model.ImgFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/business");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                model.ImgFile.CopyTo(stream);
            }

            filePath = "/uploads/business/" + fileName;
        }

        var application = new BusinessApplication
        {
            UsersId = userId.Value,
            BusinessName = model.BusinessName ?? "",
            Address = model.Address ?? "",
            Description = model.Description,
            ImgUrl = filePath,
            Status = ApplicationStatus.Bekliyor,
            IsActive = true,
            IsDelete = false,
            CreateTime = DateTime.Now
        };

        _context.BusinessApplication.Add(application);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Başvurunuz başarıyla gönderildi!";
        return RedirectToAction("MyBusiness");
    }
    
    [Route("basvurular")]
    public IActionResult Applications()
    {
        var userId = HttpContext.Session.GetInt32("Usersid");
        if (userId == null) return RedirectToAction("Login", "Login");

        var application = _context.BusinessApplication.Where(a => a.IsActive == true).ToList();
        return View(application);
    }
    [HttpPost]
    [Route("basvuru-onayla/{id}")]
    public IActionResult ApproveApplication(int id)
    {
        var application = _context.BusinessApplication.FirstOrDefault(a => a.Id == id);
        if (application == null) return NotFound();

        // Status güncelle
        application.Status = ApplicationStatus.Tamamlandı;
        _context.BusinessApplication.Update(application);
        _context.SaveChanges();

        var business = new Business
        {
            UsersId = application.UsersId,
            Name = application.BusinessName,
            Address = application.Address,
            Descrption = application.Description,
            ImgUrl = application.ImgUrl,
            IsActive = true,
            IsDelete = false,
            CreateTime = DateTime.Now
        };

        _context.Business.Add(business);

        _context.BusinessApplication.Remove(application);

        _context.SaveChanges();

        TempData["SuccessMessage"] = "Başvuru onaylandı, işletme kaydedildi.";
        return RedirectToAction("Applications"); // Admin başvuru listesine dön
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
