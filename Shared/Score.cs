namespace Shared;

public class Score
{
    public string Id { get; set; }
    public string StreamId { get; set; }
    public DateTime CreatedOnUtc { get; set; }

    public string UserName { get; set; }
    public string FullName { get; set; }
    public string IPAddress { get; set; }
    public string City { get; set; }
    public int RiskScore { get; set; }
}