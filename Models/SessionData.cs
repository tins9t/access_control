namespace ssd_authorization_solution.Models;

public class SessionData
{
    public string UserId { get; set; }
    public string Role { get; set; }
    public SessionData(string userId, string role)
    {
        UserId = userId;
        Role = role;
    }
}