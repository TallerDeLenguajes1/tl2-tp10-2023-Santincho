using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;

namespace tl2_tp10_2023_Santincho.Controllers;

public class TareaController : Controller
{
    private readonly ITareaRepository tareaRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger)
    {
        _logger = logger;
        tareaRepository = new TareaRepository();
    }

    [HttpPost("crearTarea")]
    public IActionResult Crear(int boardId, [FromForm] Tarea task)
    {
        tareaRepository.CreateTask(boardId, task);
        return RedirectToAction("Index");
    }

    [HttpGet("crearTarea")]
    public IActionResult Crear(int boardId)
    {
        Tarea tarea = new()
        {
            IdTablero = boardId
        };
        return View(tarea);
    }

    [HttpGet]
    public IActionResult Index(int userdId)
    {
        List<Tarea> tasks = tareaRepository.ListTareas();

        return View(tasks);
    } 

    [HttpGet("Tablero/{boardId}")]
    public IActionResult ListByBoard(int boardId)
    {
        List<Tarea> tasks = tareaRepository.GetTasksByBoard(boardId);

        return View(tasks);
    }

    [HttpGet("Usuario/{userId}")]
    public IActionResult ListByUser(int userId)
    {
        List<Tarea> tasks = tareaRepository.GetTasksByUser(userId);

        return View(tasks);
    } 

    [HttpPost("eliminarTarea/{id}")]
    public IActionResult Eliminar(int id)
    {
        var task = tareaRepository.GetTaskById(id);

        if (task.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        tareaRepository.DeleteTaskById(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editarTarea/{id}")]
    public IActionResult Editar(int id)
    {
        var task = tareaRepository.GetTaskById(id);

        if (task.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }
        return View(task);
    }

    [HttpPost("editarTarea/{id}")]
    public IActionResult Editar(int id, [FromForm] Tarea newTarea)
    {
        var existingTask = tareaRepository.GetTaskById(id);

        if (existingTask.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        tareaRepository.UpdateTask(id, newTarea);

        return RedirectToAction("Index");
    }
}
