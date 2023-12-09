using kanban.Models;

namespace kanban.Repository;

public interface ITableroRepository
{
    public Tablero CreateBoard(Tablero board);
    public void ModifyBoardById(int id, Tablero board);
    public List<Tablero> ListBoards();
    public Tablero GetTableroById(int id);
    public List<Tablero> GetBoardsByUser(int userId);
    public void DeleteBoardById(int id);
}