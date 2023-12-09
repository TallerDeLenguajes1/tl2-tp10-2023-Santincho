namespace kanban.Models;
public class Tablero
{
    public int Id {get; set;}
    public int IdUsuarioPropietario {get; set;}
    public string Nombre {get; set;}
    public string Descripcion {get; set;}
}