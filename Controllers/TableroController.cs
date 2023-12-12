using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tl2_tp10_2023_Santincho.Controllers;

public class TableroController : Controller
{
    private readonly ITableroRepository _tableroRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<TableroController> _logger;

    public TableroController(ILogger<TableroController> logger, ITareaRepository tareaRepository, 
    ITableroRepository tableroRepository, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _tableroRepository = tableroRepository;
        _tareaRepository = tareaRepository;
        _usuarioRepository = usuarioRepository;
    }

    [HttpPost("crearTablero")]
    public IActionResult Crear([FromForm] CrearTableroViewModel board)
    {
        try
        {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

            Tablero tablero = new() {
                Nombre = board.Nombre,
                Descripcion = board.Descripcion,
                IdUsuarioPropietario = board.IdUsuarioPropietario
            };

            if (!LoginHelper.IsAdmin(HttpContext))
            {
                tablero.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
            }

            _tableroRepository.CreateBoard(tablero);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
        
    }

    [HttpGet("crearTablero")]
    public IActionResult Crear()
    {
        try
        {
            if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

            var usuarios = _usuarioRepository.UsersList();
            var usuariosList = new SelectList(usuarios, "Id", "NombreDeUsuario");

            CrearTableroViewModel crearTableroViewModel = new() {
                IsAdmin = LoginHelper.IsAdmin(HttpContext)
            };

            ViewBag.Users = usuariosList;
            return View(crearTableroViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
        
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

            List<Tablero> boards = new();

            ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);
            
            if (ViewBag.EsAdmin) boards = _tableroRepository.ListBoards();

            else boards = _tableroRepository.GetBoardsByUser(Convert.ToInt32(LoginHelper.GetUserId(HttpContext)));

            List<ListarTablerosViewModel> tableros = new();

            foreach (var board in boards)
            {
                var user = _usuarioRepository.GetUserById(board.IdUsuarioPropietario);
                ListarTablerosViewModel tablero = new()
                {
                    Id = board.Id,
                    Nombre = board.Nombre,
                    Descripcion = board.Descripcion
                };
                if (user.Id == 0) tablero.UsuarioPropietario = "";
                else tablero.UsuarioPropietario = user.NombreDeUsuario;
                tableros.Add(tablero);
            }

            return View(tableros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("eliminarTablero/{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

            var board = _tableroRepository.GetTableroById(id);

            if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");
            
            if (!LoginHelper.IsAdmin(HttpContext))
            {
                var userBoards = _tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = userBoards.Find(board => board.Id == id);

                if (foundBoard != null)
                {
                    var tasks = _tareaRepository.GetTasksByBoard(id);
                    foreach (var task in tasks)
                    {
                        _tareaRepository.DeleteTaskById(task.Id);
                    }
                    _tableroRepository.DeleteBoardById(id);
                } else {
                    return NotFound($"No existe el tablero con ID {id}");
                }
            } else {
                var tasksToDelete = _tareaRepository.GetTasksByBoard(id);

                foreach (var task in tasksToDelete)
                {
                    _tareaRepository.DeleteTaskById(task.Id);
                }

                _tableroRepository.DeleteBoardById(id);
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
        
    }

    [HttpGet("editarTablero/{id}")]
    public IActionResult Editar(int id)
    {
        if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
        try
        {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
            
            var board = _tableroRepository.GetTableroById(id);

            if (board.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            ModificarTableroViewModel tablero = new(){
                Nombre = board.Nombre,
                Descripcion = board.Descripcion,
                Id = board.Id
            };

            if (!LoginHelper.IsAdmin(HttpContext))
            {
                var userBoards = _tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = userBoards.Find(board => board.Id == id);
                if (foundBoard == null) return NotFound($"No se encontró el tablero con ID {id}");
            } else
            {
                tablero.IdUsuarioPropietario = board.IdUsuarioPropietario;
            }

            return View(tablero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("editarTablero/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTableroViewModel newBoard)
    {
        try
        {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

            var existingBoard = _tableroRepository.GetTableroById(id);

            if (existingBoard.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            Tablero newtTablero = new() {
                IdUsuarioPropietario = newBoard.IdUsuarioPropietario == 0 ? existingBoard.IdUsuarioPropietario : newBoard.IdUsuarioPropietario,
                Nombre = newBoard.Nombre,
                Descripcion = newBoard.Descripcion,
            };

            if (!LoginHelper.IsAdmin(HttpContext))
            {
                var userBoards = _tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = userBoards.Find(board => board.Id == id);
                if (foundBoard == null) return NotFound($"No se encontró el tablero con ID {id}");
                else newtTablero.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
            }

            _tableroRepository.ModifyBoardById(id, newtTablero);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.StackTrace;
            return RedirectToAction("Error", "Home");
        }
        
    }
}
