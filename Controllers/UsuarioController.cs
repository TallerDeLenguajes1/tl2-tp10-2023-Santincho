using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;

namespace tl2_tp10_2023_Santincho.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = new UsuarioRepository();
    }

    [HttpPost("crearUsuario")]
    public IActionResult CrearUsuario([FromForm] CrearUsuarioViewModel user)
    {
        if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
        Usuario usuario = new() {
            NombreDeUsuario = user.NombreDeUsuario,
            Contrasenia = user.Contrasena,
            Rol = user.Rol
        };
        _usuarioRepository.CreateUser(usuario);
        return RedirectToAction("Index");
    }

    [HttpGet("crearUsuario")]
    public IActionResult CrearUsuario()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            return View(new CrearUsuarioViewModel());
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Usuario> users = _usuarioRepository.UsersList();

            List<ListarUsuariosViewModel> usuarios = new();

            foreach (var user in users)
            {
                ListarUsuariosViewModel usuario = new() {
                    Id = user.Id,
                    NombreDeUsuario = user.NombreDeUsuario,
                    Rol = user.Rol
                };
                usuarios.Add(usuario);
            }

            return View(usuarios);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var user = _usuarioRepository.GetUserById(id);

            if (user.Id == 0) return NotFound($"No existe el usuario con ID {id}");

            _usuarioRepository.DeleteUserById(id);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("editarUsuario/{id}")]
    public IActionResult Editar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var user = _usuarioRepository.GetUserById(id);

            if (user.Id == 0)
            {
                return NotFound($"No se encontró el usuario con ID {id}");
            }

            ModificarUsuarioViewModel usuario = new() {
                Id = user.Id,
                NombreDeUsuario = user.NombreDeUsuario,
                Contrasena = user.Contrasenia,
                Rol = user.Rol
            };

            return View(usuario);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("editarUsuario/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarUsuarioViewModel newUser)
    {
        if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
        if (LoginHelper.IsLogged(HttpContext))
        {
            var existingBoard = _usuarioRepository.GetUserById(id);

            if (existingBoard.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            Usuario newUsuario = new() {
                NombreDeUsuario = newUser.NombreDeUsuario,
                Contrasenia = newUser.Contrasena,
                Rol = newUser.Rol
            };

            _usuarioRepository.UpdateUser(newUsuario);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }
}
