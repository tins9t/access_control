namespace ssd_authorization_solution.Models;

public class SessionData
{
    public string Username { get; set; }
    public string Role { get; set; }
    public List<string> Permissions { get; set; }

    public SessionData(string username, string role, List<string> permissions)
    {
        Username = username;
        Role = role;
        Permissions = permissions;
    }
}