using Microsoft.AspNetCore.Mvc;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.ViewModels;
namespace kanban.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginController(ILogger<LoginController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = new UsuarioRepository();
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel loginModel)
    {
        var sessionUsername = LoginHelper.GetUserName(HttpContext);
        var sessionId = LoginHelper.GetUserId(HttpContext);

        if (!string.IsNullOrEmpty(sessionUsername) && !string.IsNullOrEmpty(sessionId))
        {
            var sessionUser = _usuarioRepository.GetUserById(int.Parse(sessionId));
            if (sessionUser.NombreDeUsuario == loginModel.Username)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        var userFound = _usuarioRepository.GetByUsername(loginModel.Username);

        if (userFound != null && userFound.Contrasenia == loginModel.Password)
        {
            LogInUser(userFound);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View("Index");
        }
    }

    private void LogInUser(Usuario user)
    {
        HttpContext.Session.SetString("id", user.Id.ToString());
        HttpContext.Session.SetString("usuario", user.NombreDeUsuario);
        HttpContext.Session.SetString("rol", user.Rol.ToString());
    }
}