using kanban.Models;

namespace kanban.Repository;

public interface IUsuarioRepository
{
    public void CreateUser(Usuario user);
    public void UpdateUser(Usuario user);
    public List<Usuario> UsersList();
    public Usuario GetUserById(int id);
    public void DeleteUserById(int id);
}