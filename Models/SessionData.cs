namespace ssd_authorization_solution.Models;

public class SessionData
{
    public string Username { get; set; }
    public string UserId { get; set; }
    public string Role { get; set; }
    public SessionData(string username, string userId, string role)
    {
        Username = username;
        UserId = userId;
        Role = role;
    }
}