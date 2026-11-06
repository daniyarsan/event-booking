namespace EventBooking.Domain;

public record UserId(Guid Value);

public class User
{
    public User()
    {
        
    }
    private readonly List<SocialNetwork> _socialNetworks = [];
    public UserId Id { get; init; }
    public Details Details { get; set; }
}

public class Details
{
    public string Description { get; set; }
    public IReadOnlyList<SocialNetwork> SocialNetworks {get; init; }
    
}
public record SocialNetwork
{
    public string Link;
    public string Name;
}