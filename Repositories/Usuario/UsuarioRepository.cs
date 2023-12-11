using System.Data.SqlClient;
using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string? _connectionString;
    public UsuarioRepository (string connectionString) {
        _connectionString = connectionString;
    }
    public void CreateUser(Usuario user) {
        var query = $"INSERT INTO usuario (nombre_de_usuario, contrasena, rol) VALUES (@username, @password, @rol)";
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
        command.Parameters.Add(new SQLiteParameter("@password", user.Contrasenia));
        command.Parameters.Add(new SQLiteParameter("@rol", user.Rol));

        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al crear el usuario");
        connection.Close();
    }

    public void UpdateUser(Usuario user) {
        var query = $"UPDATE usuario SET nombre_de_usuario = @username, contrasena = @password, rol = @rol WHERE id = @userId";
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
        command.Parameters.Add(new SQLiteParameter("@password", user.Contrasenia));
        command.Parameters.Add(new SQLiteParameter("@rol", user.Rol));
        command.Parameters.Add(new SQLiteParameter("@userId", user.Id));
        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Se produjo un error al crear el usuario");
        connection.Close();
    }

     public List<Usuario> UsersList() {
        var query = @"SELECT * FROM usuario";
        var users = new List<Usuario>();
        using (var connection = new SQLiteConnection(_connectionString))
        {
            var command = new SQLiteCommand(query, connection);
            connection.Open();

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new Usuario()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        NombreDeUsuario = reader["nombre_de_usuario"].ToString(),
                        Rol = (Roles)Convert.ToInt32(reader["rol"])
                    };
                    users.Add(user);
                }
            }
            connection.Close();
        }
        if (users.Count == 0 && !UsersExistInDatabase())
        {
            throw new Exception("No se pudo conseguir la lista de usuarios");
        }
        return users;
     }

     public Usuario GetUserById(int id) {
        var user = new Usuario();
        using (var connection = new SQLiteConnection(_connectionString))
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM usuario WHERE id = @userId";
            command.Parameters.Add(new SQLiteParameter("@userId", id));
            connection.Open();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    user.Id = Convert.ToInt32(reader["id"]);
                    user.NombreDeUsuario = reader["nombre_de_usuario"].ToString();
                }
            }
            connection.Close();
        }
        if (user.Id == 0) throw new Exception("No se pudo conseguir el usuario");
        return user;
     }

     public void DeleteUserById(int id) {
        var connection = new SQLiteConnection(_connectionString);
        var query = $"DELETE FROM usuario WHERE id = @id";
        connection.Open();
        var command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@id", id));
        var affectedrows = command.ExecuteNonQuery();
        if (affectedrows == 0) throw new Exception("Error al eliminar el usuario");
        connection.Close();
     }

      public Usuario GetByUsername(string username)
    {
        var user = new Usuario();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM usuario WHERE nombre_de_usuario = @nombre_de_usuario";
            command.Parameters.Add(new SQLiteParameter("@nombre_de_usuario", username));
            connection.Open();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    user.Id = Convert.ToInt32(reader["id"]);
                    user.NombreDeUsuario = reader["nombre_de_usuario"].ToString();
                    user.Rol = (Roles)Convert.ToInt32(reader["rol"]);
                    user.Contrasenia = reader["contrasena"].ToString();
                }
            }
            connection.Close();
            if (user.Id == 0) throw new Exception("No se pudo conseguir el usuario");
        }
        return user;
    }
    private bool UsersExistInDatabase()
    {
        // Realiza una consulta para verificar si existen tableros en la base de datos
        var query = @"SELECT COUNT(*) FROM tablero";
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