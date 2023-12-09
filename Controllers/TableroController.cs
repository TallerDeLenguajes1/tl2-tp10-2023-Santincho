using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;

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
    public IActionResult Crear([FromForm] Tablero board)
    {
        tableroRepository.CreateBoard(board);
        return RedirectToAction("Index");
    }

    [HttpGet("crearTablero")]
    public IActionResult Crear()
    {
        return View(new Tablero());
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Tablero> boards = tableroRepository.ListBoards();

        return View(boards);
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
        return View(board);
    }

    [HttpPost("editarTablero/{id}")]
    public IActionResult Editar(int id, [FromForm] Tablero newBoard)
    {
        var existingBoard = tableroRepository.GetTableroById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        tableroRepository.ModifyBoardById(id, newBoard);

        return RedirectToAction("Index");
    }
}
