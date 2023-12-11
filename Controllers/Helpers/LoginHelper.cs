namespace kanban.Controllers.Helpers
{
    public static class LoginHelper
    {

        public static bool IsLogged(HttpContext context)
        {
            if (context.Session.GetString("id") != null)
                return true;

            return false;
        }

        public static bool IsAdmin(HttpContext context)
        {
            if (context.Session.GetString("rol") == "Administrador")
                return true;

            return false;
        }

        public static string? GetUserId(HttpContext context)
        {
            return context.Session.GetString("id");
        }

        public static string? GetUserName(HttpContext context)
        {
            return context.Session.GetString("nombre_de_usuario");
        }

        public static string? GetUserRol(HttpContext context)
        {
            return context.Session.GetString("rol");
        }
    }
}