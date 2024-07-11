namespace EFcore.Domain;

public class Team : BaseDomainModel
{    
    //? means nullable when creating DB
    public string? Name { get; set; }

    public virtual Coach Coach { get; set; }
    public int CoachId { get; set; }

    public virtual League? League { get; set; }
    public int? LeagueId { get; set; }

    public virtual List<Match> HomeMatches { get; set; } = new List<Match>() { };
    public virtual List<Match> AwayMatches { get; set; } = new List<Match>() { };
}
