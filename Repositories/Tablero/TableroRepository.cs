using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class TableroRepository : ITableroRepository
{
    private readonly string connectionString = "Data Source=DB/kanban.db;Cache=Shared";
    public Tablero CreateBoard(Tablero board) {
        var query = $"INSERT INTO tablero (id_usuario_propietario, nombre, descripcion) VALUES (@ownerUserId, @name, @description)";
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@ownerUserId", board.IdUsuarioPropietario));
        command.Parameters.Add(new SQLiteParameter("@name", board.Nombre));
        command.Parameters.Add(new SQLiteParameter("@description", board.Descripcion));

        command.ExecuteNonQuery();

        connection.Close();

        return board;
    }
    public void ModifyBoardById(int id, Tablero board) {
        var query = $"UPDATE tablero SET id_usuario_propietario = @ownerUserId, nombre = @name, descripcion = @description WHERE id = @boardId";
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@ownerUserId", board.IdUsuarioPropietario));
        command.Parameters.Add(new SQLiteParameter("@name", board.Nombre));
        command.Parameters.Add(new SQLiteParameter("@description", board.Descripcion));
        command.Parameters.Add(new SQLiteParameter("@boardId", id));

        command.ExecuteNonQuery();
        connection.Close();
    }
    public List<Tablero> ListBoards() {
        var boards = new List<Tablero>();
        var query = @"SELECT * FROM tablero";
        using (var connection = new SQLiteConnection(connectionString))
        {
            var command = new SQLiteCommand(query, connection);
            connection.Open();

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var board = new Tablero
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString()
                    };
                    boards.Add(board);
                }
            }
            connection.Close();
        }
        return boards;
    }
    public Tablero GetTableroById(int id) {
        var board = new Tablero();
        using (var connection = new SQLiteConnection(connectionString))
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM tablero WHERE id = @id";
            command.Parameters.Add(new SQLiteParameter("@id", id));
            connection.Open();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    board.Id = Convert.ToInt32(reader["id"]);
                    board.IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]);
                    board.Nombre = reader["nombre"].ToString();
                    board.Descripcion = reader["descripcion"].ToString();
                }
            }
            connection.Close();
        }
        return board;
    }
    public List<Tablero> GetBoardsByUser(int userId) {
        var boards = new List<Tablero>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM tablero WHERE id_usuario_propietario = @ownerUserId";
            command.Parameters.Add(new SQLiteParameter("@ownerUserId", userId));
            connection.Open();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var board = new Tablero {
                        IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString()
                    };
                    boards.Add(board);
                }
            }
        }
        return boards;
    }
    public void DeleteBoardById(int id) {
        var connection = new SQLiteConnection(connectionString);
        var query = $"DELETE FROM tablero WHERE id = @id";
        connection.Open();
        var command = new SQLiteCommand(query, connection);
        command.Parameters.Add(new SQLiteParameter("@id", id));
        command.ExecuteNonQuery();
        connection.Close();
    }
}