using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;

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
    public IActionResult Crear(int boardId, [FromForm] CrearTareaViewModel task)
    {
        Tarea tarea = new(){
            Id = task.Id,
            IdTablero = task.IdTablero,
            Color = task.Color,
            Nombre = task.Nombre,
            Descripcion = task.Descripcion,
            Estado = task.Estado,
            IdUsuarioAsignado = task.IdUsuarioAsignado,

        };
        tareaRepository.CreateTask(boardId, tarea);
        return RedirectToAction("Index");
    }

    [HttpGet("crearTarea")]
    public IActionResult Crear(int boardId)
    {
        Tarea tarea = new()
        {
            IdTablero = boardId
        };

        CrearTareaViewModel task = new(){
            IdTablero = tarea.IdTablero
        };

        return View(task);
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Tarea> tasks = tareaRepository.ListTareas();

        List<ListarTareasViewModel> tareas = new();

        foreach (var task in tasks)
        {
            ListarTareasViewModel tarea = new(){
                Id = task.Id,
                IdTablero = task.IdTablero,
                Estado = task.Estado,
                Nombre = task.Nombre,
                Descripcion = task.Descripcion,
                Color = task.Color
            };
            tareas.Add(tarea);
        }

        return View(tareas);
    } 

    [HttpGet("Tablero/{boardId}")]
    public IActionResult ListByBoard(int boardId)
    {
        List<Tarea> tasks = tareaRepository.GetTasksByBoard(boardId);

        List<ListarTareasViewModel> tareas = new();

        foreach (var task in tasks)
        {
            ListarTareasViewModel tarea = new(){
                Id = task.Id,
                IdTablero = task.IdTablero,
                Estado = task.Estado,
                Nombre = task.Nombre,
                Descripcion = task.Descripcion,
                Color = task.Color
            };
            tareas.Add(tarea);
        }

        return View(tasks);
    }

    [HttpGet("Usuario/{userId}")]
    public IActionResult ListByUser(int userId)
    {
        List<Tarea> tasks = tareaRepository.GetTasksByUser(userId);

        List<ListarTareasViewModel> tareas = new();

        foreach (var task in tasks)
        {
            ListarTareasViewModel tarea = new(){
                Id = task.Id,
                IdTablero = task.IdTablero,
                Estado = task.Estado,
                Nombre = task.Nombre,
                Descripcion = task.Descripcion,
                Color = task.Color
            };
            tareas.Add(tarea);
        }

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

        ModificarTareaViewModel tarea = new(){
            Id = task.Id,
            IdTablero = task.IdTablero,
            Estado = task.Estado,
            Nombre = task.Nombre,
            Descripcion = task.Descripcion,
            Color = task.Color
        };

        return View(tarea);
    }

    [HttpPost("editarTarea/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTareaViewModel newTarea)
    {
        Tarea newTask = new(){
            Id = newTarea.Id,
            IdTablero = newTarea.IdTablero,
            Color = newTarea.Color,
            Nombre = newTarea.Nombre,
            Descripcion = newTarea.Descripcion,
            Estado = newTarea.Estado,
            IdUsuarioAsignado = newTarea.IdUsuarioAsignado,

        };
        
        var existingTask = tareaRepository.GetTaskById(id);

        if (existingTask.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        tareaRepository.UpdateTask(id, newTask);

        return RedirectToAction("Index");
    }
}
