using System.ComponentModel.DataAnnotations;
namespace kanban.ViewModels;
public class CrearTableroViewModel
{
    [Required(ErrorMessage = "Complete el campo")]
    public int IdUsuarioPropietario { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public bool IsAdmin { get; set; }
}