using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppointmentSystem.Models;
using AppointmentSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppointmentSystem.Entities;

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
        if (userId == null) return RedirectToAction("Login","Login");
        var appointment = _context.Appointments
        .Include(vm => vm.Users)
        .Include(vm => vm.Business)
        .Where(vm => vm.IsActive == true && vm.IsDelete == false && vm.UsersId == userId)
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


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
