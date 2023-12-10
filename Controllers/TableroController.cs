using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;

namespace tl2_tp10_2023_Santincho.Controllers;

public class TableroController : Controller
{
    private readonly ITableroRepository tableroRepository;
    private readonly ITareaRepository tareaRepository;
    private readonly ILogger<TableroController> _logger;

    public TableroController(ILogger<TableroController> logger)
    {
        _logger = logger;
        tableroRepository = new TableroRepository();
        tareaRepository = new TareaRepository();
    }

    [HttpPost("crearTablero")]
    public IActionResult Crear([FromForm] CrearTableroViewModel board)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        Tablero tablero = new() {
            Nombre = board.Nombre,
            Descripcion = board.Descripcion,
        };

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            tablero.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
        }

        tableroRepository.CreateBoard(tablero);

        return RedirectToAction("Index");
    }

    [HttpGet("crearTablero")]
    public IActionResult Crear()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        CrearTableroViewModel crearTableroViewModel = new() {
            IsAdmin = LoginHelper.IsAdmin(HttpContext)
        };
        return View(crearTableroViewModel);
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        List<Tablero> boards = new();

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);
        
        if (ViewBag.EsAdmin) boards = tableroRepository.ListBoards();

        else boards = tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));

        List<ListarTablerosViewModel> tableros = new();

        foreach (var board in boards)
        {
            ListarTablerosViewModel tablero = new(board.Nombre, board.Descripcion, board.Id, board.IdUsuarioPropietario);
            tableros.Add(tablero);
        }

        return View(tableros);
    }

    [HttpPost("eliminarTablero/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = tableroRepository.GetTableroById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");
        
        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);

            if (foundBoard != null)
            {
                var tasks = tareaRepository.GetTasksByBoard(id);
                foreach (var task in tasks)
                {
                    tareaRepository.DeleteTaskById(task.Id);
                }
                tableroRepository.DeleteBoardById(id);
            } else {
                return NotFound($"No existe el tablero con ID {id}");
            }
        } else {
            var tasksToDelete = tareaRepository.GetTasksByBoard(id);

            foreach (var task in tasksToDelete)
            {
                tareaRepository.DeleteTaskById(task.Id);
            }

            tableroRepository.DeleteBoardById(id);
        }

        return RedirectToAction("Index");
    }

    [HttpGet("editarTablero/{id}")]
    public IActionResult Editar(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
        var board = tableroRepository.GetTableroById(id);

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
            var userBoards = tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard == null) return NotFound($"No se encontró el tablero con ID {id}");
        } else
        {
            tablero.IdUsuarioPropietario = board.IdUsuarioPropietario;
        }

        return View(tablero);
    }

    [HttpPost("editarTablero/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTableroViewModel newBoard)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var existingBoard = tableroRepository.GetTableroById(id);

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
            var userBoards = tableroRepository.GetBoardsByUser(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard == null) return NotFound($"No se encontró el tablero con ID {id}");
            else newtTablero.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
        }

        tableroRepository.ModifyBoardById(id, newtTablero);

        return RedirectToAction("Index");
    }
}
