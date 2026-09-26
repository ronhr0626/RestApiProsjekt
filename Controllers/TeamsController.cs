using Microsoft.AspNetCore.Mvc;
using Oppgave4Api.Model;
using Oppgave4Api.Services;

namespace Oppgave4Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly TeamService _service;

    public TeamsController(TeamService service)
    {
        _service = service;
    }

    // GET /api/teams  or  GET /api/teams?name=Brann
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Ok(await _service.GetAllAsync());

        var team = await _service.GetByNameAsync(name);
        if (team == null)
            return Problem(statusCode: 404, title: $"Team '{name}' not found");
        return Ok(team);
    }

    // GET /api/teams/3
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var team = await _service.GetByIdAsync(id);
        if (team == null)
            return Problem(statusCode: 404, title: $"No team with id {id}");
        return Ok(team);
    }

    // POST /api/teams
    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamDto dto)
    {
        if (await _service.GetByNameAsync(dto.Name) != null)
            return Problem(statusCode: 409, title: $"Team '{dto.Name}' already exists");

        var team = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }
}