using System.Data.SqlClient;
using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string connectionString = "Data Source=DB/kanban.db;Cache=Shared";
    public void CreateUser(Usuario user) {
        var query = $"INSERT INTO usuario (nombre_de_usuario) VALUES (@username)";
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));

        command.ExecuteNonQuery();

        connection.Close();
    }

    public void UpdateUser(Usuario user) {
        var query = $"UPDATE usuario SET nombre_de_usuario = @username WHERE id = @userId";
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
        command.Parameters.Add(new SQLiteParameter("@userId", user.Id));
        command.ExecuteNonQuery();
        connection.Close();
    }

     public List<Usuario> UsersList() {
        var query = @"SELECT * FROM usuario";
        var users = new List<Usuario>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            var command = new SQLiteCommand(query, connection);
            connection.Open();

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new Usuario
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        NombreDeUsuario = reader["nombre_de_usuario"].ToString()
                    };
                    users.Add(user);
                }
            }
            connection.Close();
        }
        return users;
     }

     public Usuario GetUserById(int id) {
        var user = new Usuario();
        using (var connection = new SQLiteConnection(connectionString))
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
        return user;
     }

     public void DeleteUserById(int id) {
        var connection = new SQLiteConnection(connectionString);
        var query = $"DELETE FROM usuario WHERE id = @id";
        connection.Open();
        var command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@id", id));
        command.ExecuteNonQuery();
        connection.Close();
     }
}