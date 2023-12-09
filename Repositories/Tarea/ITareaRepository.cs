using kanban.Models;

namespace kanban.Repository;

public interface ITareaRepository
{
    public Tarea CreateTask(int boardId, Tarea task);
    public void UpdateTask(int taskId, Tarea task);
    public Tarea GetTaskById(int id);
    public List<Tarea> ListTareas();
    public List<Tarea> GetTasksByUser(int userId);
    public List<Tarea> GetTasksByBoard(int boardId);
    public void DeleteTaskById(int id);
    public void AssingUserToTask(int userId, int taskId);
}