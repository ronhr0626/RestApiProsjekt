using Oppgave4Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();           // same error format everywhere
builder.Services.AddSingleton<TeamService>();   // one shared list for the whole app

var app = builder.Build();

app.UseExceptionHandler();   // crashes -> ProblemDetails
app.UseStatusCodePages();    // empty error codes -> ProblemDetails

app.MapOpenApi();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Oppgave4Api"));
app.MapControllers();

app.Run();