using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class TareaRepository : ITareaRepository
{
    private readonly string? _connectionString;
    public TareaRepository (string connectionString) {
        _connectionString = connectionString;
    }
    public Tarea CreateTask(int boardId, Tarea task) {
        var query = $"INSERT INTO tarea (id_tablero, nombre, estado, descripcion, color, id_usuario_asignado) VALUES (@id_tablero, @name, @estado, @descripcion, @color, @usuario)";
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@id_tablero", boardId));
        command.Parameters.Add(new SQLiteParameter("@name", task.Nombre));
        command.Parameters.Add(new SQLiteParameter("@estado", task.Estado));
        command.Parameters.Add(new SQLiteParameter("@descripcion", task.Descripcion));
        command.Parameters.Add(new SQLiteParameter("@color", task.Color));
        command.Parameters.Add(new SQLiteParameter("@usuario", task.IdUsuarioAsignado));

        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al crear la tarea");

        connection.Close();
        return task;
    }
    public void UpdateTask(int taskId, Tarea task) {
        var queryString = @"UPDATE tarea SET nombre = @name, estado = @status, descripcion = @description, color = @color WHERE id = @taskId";
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = new SQLiteCommand(queryString, connection);

        command.Parameters.Add(new SQLiteParameter("@taskId", taskId));
        command.Parameters.Add(new SQLiteParameter("@name", task.Nombre));
        command.Parameters.Add(new SQLiteParameter("@status", task.Estado));
        command.Parameters.Add(new SQLiteParameter("@description", task.Descripcion));
        command.Parameters.Add(new SQLiteParameter("@color", task.Color));
        
        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al modificar la tarea");

        connection.Close();
    }

    public Tarea GetTaskById(int id) {
        var task = new Tarea();
        using var connection = new SQLiteConnection(_connectionString);
        var command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "SELECT * FROM tarea WHERE id = @idTask";
        command.Parameters.Add(new SQLiteParameter("@idTask", id));
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                task.Id = Convert.ToInt32(reader["id"]);
                task.IdTablero = Convert.ToInt32(reader["id_tablero"]);
                task.Nombre = reader["nombre"].ToString();
                task.Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]);
                task.Descripcion = reader["descripcion"].ToString();
                task.Color = reader["color"].ToString();
                task.IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"]);
            }
        }
        if (task.Id == 0) throw new Exception("Error al encontrar la tarea");
        connection.Close();
        return task;
    }

    public List<Tarea> GetTasksByUser(int userId) {
        try
        {
            var tasks = new List<Tarea>();
            var query =  @"SELECT * FROM tarea WHERE id_usuario_asignado = @userId";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@userId", userId));
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        var task = new Tarea()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            Nombre = reader["nombre"].ToString(),
                            Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                            Descripcion = reader["descripcion"].ToString(),
                            Color = reader["color"].ToString(),
                            IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"])
                        };
                        tasks.Add(task);
                    }
                }
                connection.Close();
            }
            return tasks;
        }
        catch (System.Exception)
        {
            throw new Exception("No se pudo conseguir la lista de tareas");
        }
    }

    public List<Tarea> GetTasksByBoard(int boardId) {
        var tasks = new List<Tarea>();
        try
        {
            var query =  @"SELECT * FROM tarea WHERE id_tablero = @boardId";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(query, connection);
                command.Parameters.Add(new SQLiteParameter("@boardId", boardId));
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        var task = new Tarea()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            Nombre = reader["nombre"].ToString(),
                            Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                            Descripcion = reader["descripcion"].ToString(),
                            Color = reader["color"].ToString(),
                            IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"])
                        };
                        tasks.Add(task);
                    }
                }
                connection.Close();
            }
            return tasks;
        }
        catch (System.Exception)
        {
            throw new Exception("No se pudo conseguir la lista de tareas");
        }
    }

    public void DeleteTaskById(int id) {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM tarea WHERE id = @taskId";
        command.Parameters.Add(new SQLiteParameter("@taskId", id));
        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al eliminar la tarea");
        connection.Close();
    }

    public void AssingUserToTask(int userId, int taskId) {
        var queryString = @"UPDATE tarea SET id_usuario_asignado = @userId WHERE id = @taskId";
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = new SQLiteCommand(queryString, connection);

        command.Parameters.Add(new SQLiteParameter("@taskId", taskId));
        command.Parameters.Add(new SQLiteParameter("@userId", userId));
        
        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al asignar la tarea");

        connection.Close();
    }

    public List<Tarea> ListTareas() {
        try
        {
            var tasks = new List<Tarea>();
            var query =  @"SELECT * FROM tarea";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(query, connection);
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        var task = new Tarea()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            Nombre = reader["nombre"].ToString(),
                            Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                            Descripcion = reader["descripcion"].ToString(),
                            Color = reader["color"].ToString(),
                            IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"])
                        };
                        tasks.Add(task);
                    }
                }
                connection.Close();
            }
            return tasks;
        }
        catch (System.Exception)
        {
            throw new Exception("No se pudo conseguir la lista de tareas");
        }
    }

    private bool TasksExistInDatabase()
    {
        // Realiza una consulta para verificar si existen tableros en la base de datos
        var query = @"SELECT COUNT(*) FROM tarea";
        using (var connection = new SQLiteConnection(_connectionString))
        {
            var command = new SQLiteCommand(query, connection);
            connection.Open();

            int count = (int)command.ExecuteScalar();

            connection.Close();

            return count > 0;
        }
    }
}