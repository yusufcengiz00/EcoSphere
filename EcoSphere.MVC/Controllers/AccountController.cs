using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;
using EcoSphere.Core.Session;
using Serilog;

namespace EcoSphere.MVC.Controllers;

public class AccountController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (CurrentSession.IsLoggedIn)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Kullanıcı adı ve şifre boş bırakılamaz.";
            return View();
        }

        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || user.Password != password) // Plain text password check as requested for basic level
        {
            Log.Warning("Hatalı giriş denemesi: Kullanıcı Adı = {Username}", username);
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // Set static session
        CurrentSession.CurrentUser = user;
        Log.Information("Kullanıcı başarıyla giriş yaptı: {FullName} ({Role})", user.FullName, user.Role);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        if (CurrentSession.CurrentUser != null)
        {
            Log.Information("Kullanıcı çıkış yaptı: {Username}", CurrentSession.CurrentUser.Username);
        }
        CurrentSession.Clear();
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string forgotUsername, string forgotFullName, string forgotNewPassword)
    {
        if (string.IsNullOrEmpty(forgotUsername) || string.IsNullOrEmpty(forgotFullName) || string.IsNullOrEmpty(forgotNewPassword))
        {
            TempData["Error"] = "Tüm alanları doldurmanız gerekmektedir.";
            return RedirectToAction("Login");
        }

        var user = await _unitOfWork.Users.GetByUsernameAsync(forgotUsername);
        if (user == null || user.FullName.Trim().ToLower() != forgotFullName.Trim().ToLower())
        {
            TempData["Error"] = "Girilen bilgiler sistemdeki kayıtlarla eşleşmedi!";
            return RedirectToAction("Login");
        }

        user.Password = forgotNewPassword;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
        Log.Information("Kullanıcı şifresini sıfırladı: {Username}", forgotUsername);

        TempData["Success"] = "Şifreniz başarıyla güncellendi! Yeni şifrenizle giriş yapabilirsiniz.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (CurrentSession.IsLoggedIn)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user, string adminCode)
    {
        if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
        {
            ViewBag.Error = "Kullanıcı adı ve şifre zorunludur.";
            return View();
        }

        var existing = await _unitOfWork.Users.GetByUsernameAsync(user.Username);
        if (existing != null)
        {
            ViewBag.Error = "Bu kullanıcı adı zaten alınmış!";
            return View(user);
        }

        if (user.Role == "Admin")
        {
            if (adminCode != "ECO-2026")
            {
                ViewBag.Error = "Geçersiz Yönetici Yetkilendirme Kodu!";
                return View(user);
            }
            user.Department = "Yönetim";
        }

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        Log.Information("Yeni kullanıcı kaydoldu: {FullName} ({Role})", user.FullName, user.Role);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
