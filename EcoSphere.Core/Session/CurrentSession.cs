using EcoSphere.Core.Models;

namespace EcoSphere.Core.Session;

public static class CurrentSession
{
    public static User? CurrentUser { get; set; }
    
    public static bool IsLoggedIn => CurrentUser != null;
    public static bool IsAdmin => CurrentUser != null && CurrentUser.Role == "Admin";
    public static bool IsUser => CurrentUser != null && CurrentUser.Role == "User";
    
    public static void Clear()
    {
        CurrentUser = null;
    }
}
