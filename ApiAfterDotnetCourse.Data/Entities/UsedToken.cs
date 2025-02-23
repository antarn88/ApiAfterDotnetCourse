namespace ApiAfterDotnetCourse.Data.Entities;

public class UsedToken
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime UsedAt { get; set; }
}
