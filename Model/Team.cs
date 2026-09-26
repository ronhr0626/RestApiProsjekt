using System.ComponentModel.DataAnnotations;

namespace Oppgave4Api.Model;

// The team as it is stored in our list
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Stadium { get; set; }
    public int? FormedYear { get; set; }
    public string Source { get; set; } = "";      // status: "Seed" or "Manual"
    public DateTime CreatedAt { get; set; }        // timestamp
}

// What the user is allowed to send in with POST
public class CreateTeamDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";

    public string? Stadium { get; set; }

    [Range(1850, 2100)]
    public int? FormedYear { get; set; }
}