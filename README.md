# Oppgave4Api – Eliteserien teams

This is my REST API for assignment 4. It keeps track of football teams from Eliteserien.

When the app starts, it already has a list of 10 teams. You can look at the teams with GET and add new ones with POST.

I left Viking out of the start list on purpose. That way I can show a 404 when you search for it, and then add it with POST.

## How to run it

You need .NET 10. Open the folder in a terminal and run:

```
dotnet run
```

Then open http://localhost:5113/swagger in the browser. Everything can be tested from there.

## Files

- `Model/Team.cs` – what a team looks like, and what you are allowed to send in with POST
- `Services/TeamService.cs` – the list with the start teams, and the logic
- `Controllers/TeamsController.cs` – the endpoints
- `Program.cs` – setup

A team has an id, name, stadium, year it was founded, a source (`Seed` for the start teams, `Manual` for teams added with POST) and when it was added.

## Start teams

| Id | Team |
|---|---|
| 1 | Aalesund |
| 2 | Bodø/Glimt |
| 3 | Brann |
| 4 | Fredrikstad |
| 5 | HamKam |
| 6 | KFUM Oslo |
| 7 | Kristiansund |
| 8 | Lillestrøm |
| 9 | Molde |
| 10 | Rosenborg |

## Endpoints

**GET /api/teams**
Gets all teams.

**GET /api/teams/{id}**
Gets one team by id. Brann is id 3. If the id does not exist you get 404.

**GET /api/teams?name={name}**
Filters on name and gives you that team. It doesn't care about upper or lower case. If the team is not there you get 404.

**POST /api/teams**
Adds a new team. Example body:

```json
{ "name": "Viking", "stadium": "SR-Bank Arena", "formedYear": 1899 }
```

The name is required and must be 2–100 characters. The year has to be between 1850 and 2100.
If it works you get 201 Created and a link to the new team. If the name is missing you get 400. If the team already exists you get 409.

All errors come back in the same format (ProblemDetails), for example:

```json
{ "title": "Team 'Viking' not found", "status": 404 }
```

## How to test it quickly

Start the app and open Swagger, or use the `Oppgave4Api.http` file. Do these in order:

1. `GET /api/teams` → 200, 10 teams
2. `GET /api/teams/3` → 200, Brann
3. `GET /api/teams/999` → 404
4. `GET /api/teams?name=Viking` → 404, Viking is not in the list
5. `POST /api/teams` with `{ "name": "" }` → 400, name is required
6. `POST /api/teams` with `{ "name": "Viking" }` → 201, gets id 11
7. `GET /api/teams?name=Viking` → 200, now it's there
8. `POST /api/teams` with `{ "name": "Viking" }` again → 409, already exists

Or with curl:

```
curl http://localhost:5210/api/teams
curl -X POST http://localhost:5210/api/teams -H "Content-Type: application/json" -d "{\"name\":\"Viking\"}"
```

## Why there is no paging or sorting

I thought about paging and sorting, but I chose not to build it. Eliteserien only has 16 teams, so the list will never be big enough to need pages. Keeping the GET endpoint simple made the code easier to read and test.

If the list grew, this is how I would add it. The controller takes the values from the URL:

```csharp
[HttpGet]
public async Task<IActionResult> Get(string? sort, int page = 1, int pageSize = 20)
{
    if (page < 1 || pageSize < 1 || pageSize > 100)
        return Problem(statusCode: 400, title: "Invalid page or pageSize");

    return Ok(await _service.GetAllAsync(sort, page, pageSize));
}
```

And the service sorts the whole list first, then picks out one page:

```csharp
public Task<List<Team>> GetAllAsync(string? sort, int page, int pageSize)
{
    IEnumerable<Team> result = sort switch
    {
        "name" => _teams.OrderBy(t => t.Name),
        "year" => _teams.OrderBy(t => t.FormedYear),
        _      => _teams.OrderBy(t => t.Id)
    };

    // Skip the pages before, then take one page
    result = result.Skip((page - 1) * pageSize).Take(pageSize);
    return Task.FromResult(result.ToList());
}
```

It is important to sort first and split into pages after. Otherwise you only sort the teams on one page, not the whole list.

## About async

All endpoints are async and use await.

The methods in TeamService return Task even though the list is just in memory. I did that so I can switch to a real database later without changing the controller. I don't use .Result or .Wait() anywhere.

## Things I know are missing

- Everything is stored in memory, so teams you add are gone when the app stops. Restart the app to get the 10 start teams back.
- The list is not thread-safe. It's fine for testing, but for real use I would use a database.