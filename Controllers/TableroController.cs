using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
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
        Tablero tablero = new() {
            IdUsuarioPropietario = board.IdUsuarioPropietario,
            Nombre = board.Nombre,
            Descripcion = board.Descripcion,
            isAdmin = board.IsAdmin
        };

        tableroRepository.CreateBoard(tablero);

        return RedirectToAction("Index");
    }

    [HttpGet("crearTablero")]
    public IActionResult Crear()
    {
        return View(new CrearTableroViewModel());
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Tablero> boards = tableroRepository.ListBoards();
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
        var board = tableroRepository.GetTableroById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        var tasksToDelete = tareaRepository.GetTasksByBoard(id);

        foreach (var task in tasksToDelete)
        {
            tareaRepository.DeleteTaskById(task.Id);
        }

        tableroRepository.DeleteBoardById(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editarTablero/{id}")]
    public IActionResult Editar(int id)
    {
        var board = tableroRepository.GetTableroById(id);

        if (board.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        ModificarTableroViewModel tablero = new(){
            IdUsuarioPropietario = board.IdUsuarioPropietario,
            Nombre = board.Nombre,
            Descripcion = board.Descripcion,
            Id = board.Id
        };

        return View(tablero);
    }

    [HttpPost("editarTablero/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTableroViewModel newBoard)
    {
        var existingBoard = tableroRepository.GetTableroById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        Tablero newtTablero = new() {
            IdUsuarioPropietario = newBoard.IdUsuarioPropietario,
            Nombre = newBoard.Nombre,
            Descripcion = newBoard.Descripcion,
        };

        tableroRepository.ModifyBoardById(id, newtTablero);

        return RedirectToAction("Index");
    }
}
