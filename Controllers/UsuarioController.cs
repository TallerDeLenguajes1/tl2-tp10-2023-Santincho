using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;

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
    public IActionResult CrearUsuario([FromForm] Usuario user)
    {
        usuarioRepository.CreateUser(user);
        return RedirectToAction("Index");
    }

    [HttpGet("crearUsuario")]
    public IActionResult CrearUsuario()
    {
        return View(new Usuario());
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Usuario> users = usuarioRepository.UsersList();

        return View(users);
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
        return View(user);
    }

    [HttpPost("editarUsuario/{id}")]
    public IActionResult Editar(int id, [FromForm] Usuario newUser)
    {
        var existingBoard = usuarioRepository.GetUserById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        usuarioRepository.UpdateUser(newUser);

        return RedirectToAction("Index");
    }
}
