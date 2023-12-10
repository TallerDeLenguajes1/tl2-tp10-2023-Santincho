using System.ComponentModel.DataAnnotations;
namespace kanban.ViewModels;
public class LoginViewModel
{
    [Required(ErrorMessage = "Complete el campo")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public string Password { get; set; }
}