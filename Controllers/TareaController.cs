using System.Diagnostics;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using tl2_tp10_2023_Santincho.Models;
using kanban.ViewModels;

namespace tl2_tp10_2023_Santincho.Controllers;

public class TareaController : Controller
{
    private readonly ITareaRepository _tareaRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger, ITareaRepository tareaRepository)
    {
        _logger = logger;
        _tareaRepository = tareaRepository;
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
        _tareaRepository.CreateTask(boardId, tarea);
        return RedirectToAction("Index");
    }

    [HttpGet("crearTarea")]
    public IActionResult Crear(int boardId)
    {
        if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
        if (LoginHelper.IsLogged(HttpContext))
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
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Tarea> tasks = _tareaRepository.ListTareas();

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
        return RedirectToAction("Index", "Login");
    } 

    [HttpGet("Tablero/{boardId}")]
    public IActionResult ListByBoard(int boardId)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Tarea> tasks = _tareaRepository.GetTasksByBoard(boardId);

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
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("Usuario/{userId}")]
    public IActionResult ListByUser(int userId)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Tarea> tasks = _tareaRepository.GetTasksByUser(userId);

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
        return RedirectToAction("Index", "Login");
    } 

    [HttpPost("eliminarTarea/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var task = _tareaRepository.GetTaskById(id);

            if (task.Id == 0) return NotFound($"No existe el tablero con ID {id}");

            _tareaRepository.DeleteTaskById(id);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("editarTarea/{id}")]
    public IActionResult Editar(int id)
    {
        if(!ModelState.IsValid) return RedirectToAction("Index", "Home");
        if (LoginHelper.IsLogged(HttpContext))
        {
            var task = _tareaRepository.GetTaskById(id);

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
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("editarTarea/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTareaViewModel newTarea)
    {
        if (LoginHelper.IsLogged(HttpContext))
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
            
            var existingTask = _tareaRepository.GetTaskById(id);

            if (existingTask.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            _tareaRepository.UpdateTask(id, newTask);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }
}
