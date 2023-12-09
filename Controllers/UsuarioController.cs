using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;

namespace tl2_tp10_2023_Santincho.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
        usuarioRepository = new UsuarioRepository();
    }

    [HttpPost("crearUsuario")]
    public IActionResult CrearUsuario([FromForm] CrearUsuarioViewModel user)
    {
        Usuario usuario = new() {
            NombreDeUsuario = user.NombreDeUsuario,
            Contrasenia = user.Contrasena,
            Rol = user.Rol
        };
        usuarioRepository.CreateUser(usuario);
        return RedirectToAction("Index");
    }

    [HttpGet("crearUsuario")]
    public IActionResult CrearUsuario()
    {
        return View(new CrearUsuarioViewModel());
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Usuario> users = usuarioRepository.UsersList();

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

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        var user = usuarioRepository.GetUserById(id);

        if (user.Id == 0) return NotFound($"No existe el usuario con ID {id}");

        usuarioRepository.DeleteUserById(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editarUsuario/{id}")]
    public IActionResult Editar(int id)
    {
        var user = usuarioRepository.GetUserById(id);

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

    [HttpPost("editarUsuario/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarUsuarioViewModel newUser)
    {
        var existingBoard = usuarioRepository.GetUserById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        Usuario newUsuario = new() {
            NombreDeUsuario = newUser.NombreDeUsuario,
            Contrasenia = newUser.Contrasena,
            Rol = newUser.Rol
        };

        usuarioRepository.UpdateUser(newUsuario);

        return RedirectToAction("Index");
    }
}
