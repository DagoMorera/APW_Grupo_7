namespace APW.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SourceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
    public Source Source { get; set; }
}