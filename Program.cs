using WorkflowApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Set Kestrel to listen on port 5028
builder.WebHost.UseUrls("http://localhost:5028");

// Add CORS
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// In-memory stores
var workflowDefinitions = new List<WorkflowDefinition>();
var workflowInstances = new List<WorkflowInstance>();

// Create a new workflow definition
app.MapPost("/workflows", (WorkflowDefinition def) => {
    if (def.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Workflow must have exactly one initial state.");
    if (def.States.Select(s => s.Id).Distinct().Count() != def.States.Count)
        return Results.BadRequest("Duplicate state IDs.");
    if (def.Actions.Select(a => a.Id).Distinct().Count() != def.Actions.Count)
        return Results.BadRequest("Duplicate action IDs.");
    workflowDefinitions.Add(def);
    return Results.Ok(def);
});

// Get a workflow definition
app.MapGet("/workflows/{id}", (string id) => {
    var def = workflowDefinitions.FirstOrDefault(w => w.Id == id);
    return def is null ? Results.NotFound() : Results.Ok(def);
});

// Start a new workflow instance
app.MapPost("/workflows/{id}/instances", (string id) => {
    var def = workflowDefinitions.FirstOrDefault(w => w.Id == id);
    if (def is null) return Results.NotFound();
    var initial = def.States.First(s => s.IsInitial);
    var instance = new WorkflowInstance {
        Id = Guid.NewGuid().ToString(),
        DefinitionId = def.Id,
        CurrentState = initial.Id,
        History = new List<InstanceHistory>()
    };
    workflowInstances.Add(instance);
    return Results.Ok(instance);
});

// Execute an action on an instance
app.MapPost("/instances/{instanceId}/actions/{actionId}", (string instanceId, string actionId) => {
    var instance = workflowInstances.FirstOrDefault(i => i.Id == instanceId);
    if (instance is null) return Results.NotFound();
    var def = workflowDefinitions.FirstOrDefault(w => w.Id == instance.DefinitionId);
    if (def is null) return Results.NotFound();
    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action is null) return Results.BadRequest("Action not found.");
    if (!action.Enabled) return Results.BadRequest("Action is disabled.");
    if (!action.FromStates.Contains(instance.CurrentState))
        return Results.BadRequest("Action not allowed from current state.");
    var currentState = def.States.First(s => s.Id == instance.CurrentState);
    if (currentState.IsFinal) return Results.BadRequest("Cannot act on final state.");
    var toState = def.States.FirstOrDefault(s => s.Id == action.ToState);
    if (toState is null) return Results.BadRequest("Target state does not exist.");
    if (!toState.Enabled) return Results.BadRequest("Target state is disabled.");
    var history = new InstanceHistory {
        ActionId = action.Id,
        FromState = instance.CurrentState,
        ToState = toState.Id,
        Timestamp = DateTime.UtcNow
    };
    instance.CurrentState = toState.Id;
    instance.History.Add(history);
    return Results.Ok(instance);
});

// Get instance state and history
app.MapGet("/instances/{instanceId}", (string instanceId) => {
    var instance = workflowInstances.FirstOrDefault(i => i.Id == instanceId);
    return instance is null ? Results.NotFound() : Results.Ok(instance);
});

app.MapGet("/", () => "Hello World!");

app.Run();
