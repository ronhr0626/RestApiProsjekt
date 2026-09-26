using Oppgave4Api.Model;

namespace Oppgave4Api.Services;

public class TeamService
{
    private readonly List<Team> _teams = new();
    private int _nextId = 1;

    // Start data: 10 Eliteserien teams. Viking is left out on purpose (used to test 404 and POST).
    public TeamService()
    {
        AddStartTeam("Aalesund", "Color Line Stadion", 1914);
        AddStartTeam("Bodø/Glimt", "Aspmyra Stadion", 1916);
        AddStartTeam("Brann", "Brann Stadion", 1908);
        AddStartTeam("Fredrikstad", "Fredrikstad Stadion", 1903);
        AddStartTeam("HamKam", "Briskeby Stadion", 1918);
        AddStartTeam("KFUM Oslo", "KFUM Arena", 1939);
        AddStartTeam("Kristiansund", "Kristiansund Stadion", 2003);
        AddStartTeam("Lillestrøm", "Åråsen Stadion", 1917);
        AddStartTeam("Molde", "Aker Stadion", 1911);
        AddStartTeam("Rosenborg", "Lerkendal Stadion", 1917);
    }

    private void AddStartTeam(string name, string stadium, int formedYear)
    {
        _teams.Add(new Team
        {
            Id = _nextId++,
            Name = name,
            Stadium = stadium,
            FormedYear = formedYear,
            Source = "Seed",
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<List<Team>> GetAllAsync() =>
        Task.FromResult(_teams.ToList());

    public Task<Team?> GetByIdAsync(int id) =>
        Task.FromResult(_teams.FirstOrDefault(t => t.Id == id));

    public Task<Team?> GetByNameAsync(string name) =>
        Task.FromResult(_teams.FirstOrDefault(t =>
            t.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)));

    public Task<Team> CreateAsync(CreateTeamDto dto)
    {
        var team = new Team
        {
            Id = _nextId++,
            Name = dto.Name.Trim(),
            Stadium = dto.Stadium,
            FormedYear = dto.FormedYear,
            Source = "Manual",
            CreatedAt = DateTime.UtcNow
        };
        _teams.Add(team);
        return Task.FromResult(team);
    }
}