using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeysShop.Data;
using KeysShop.Models;

namespace KeysShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Страница регистрации
        public IActionResult Register()
        {
            return View();
        }

        // Обработка регистрации с BCrypt
        [HttpPost]
        public async Task<IActionResult> Register(User user, string confirmPassword)
        {
            // Проверка совпадения паролей
            if (user.Password != confirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View(user);
            }

            // Проверяем, нет ли пользователя с таким email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                ViewBag.Error = "Пользователь с таким email уже есть";
                return View(user);
            }

            // Хешируем пароль с помощью BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "User";

            // Сохраняем в БД
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Сохраняем в сессии
            HttpContext.Session.SetInt32("UserId", user.id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Catalog");
        }

        // Страница входа
        public IActionResult Login()
        {
            return View();
        }

        // Обработка входа с BCrypt
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Ищем пользователя по email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Пользователь не найден";
                return View();
            }

            // Проверяем пароль с помощью BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isPasswordValid)
            {
                ViewBag.Error = "Неверный пароль";
                return View();
            }

            // Сохраняем в сессии
            HttpContext.Session.SetInt32("UserId", user.id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Catalog");
        }

        // Выход
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Catalog");
        }

        // Профиль
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }
    }
}