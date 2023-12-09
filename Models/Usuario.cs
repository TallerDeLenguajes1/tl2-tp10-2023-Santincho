namespace kanban.Models;

public enum Roles {
    Administrador,
    Operador
}
public class Usuario
{
    public int Id {get; set;}
    public string NombreDeUsuario {get; set;}
    public string Contrasenia {get; set;}
    public Roles Rol {get; set;}
}