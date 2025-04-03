using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Client.Models;
using RequestManagement.Client.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RequestManagement.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly GrpcAuthService _authService;

        public AccountController(GrpcAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous] // Доступ без аутентификации
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.AuthenticateAsync(model.Login, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный логин или пароль.");
                return View(model);
            }

            // Создание claims для аутентификации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, ((RequestManagement.Common.Models.Enums.UserRole)user.Role).ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Сохранять cookie после закрытия браузера
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Срок действия совпадает с JWT
            };

            // Вход в систему
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Сохранение JWT-токена (опционально, если нужен для gRPC-запросов)
            HttpContext.Session.SetString("JwtToken", user.Token);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Request");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}