using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MmReddit.Model;
using MmReddit.Service;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);

// Opretter en instans af WebApplication og konfigurerer den.

// Sætter CORS, så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Tilføj DbContext-fabrik som service.
builder.Services.AddDbContext<MmRedditContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilføj DataService som en tjeneste, så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

var app = builder.Build();

// Konfigurerer applikationen

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

// Opretter scope for at udføre datainitialisering
using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData();
}

using (var db = new MmRedditContext())
{
    // Initialiserer en databasekontekst
}

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// Definerer endpoint-ruter

app.MapGet("/api/posts", (DataService service) =>
{
    return service.GetPosts();
});

// Henter en bruger på dets id 
app.MapGet("/api/user/{id}", (DataService services, int id) =>
{
    return services.GetUserId(id);
});

// Opretter en ny post
app.MapPost("/api/post/", (DataService service, PostData data) =>
{
    return service.CreatePost(data.title, data.user, data.content, data.upvotes, data.downvotes, data.numberOfVotes, data.postTime);
});

// Opretter en ny kommentar på et spcifikt post
app.MapPost("/api/post/{id}/comment", (DataService service, CommentData data, int id) =>
{
    return service.CreateComment(data.content, data.upvotes, data.downvotes, data.numberOfVotes, id, data.user, data.commentTime);
});

// Laver en upvote på et specifikt post
app.MapPut("/api/post/{id}/upvote", (DataService service, int id) =>
{
    return service.PostUpvote(id);
});

// Laver et downvote på et specifikt post
app.MapPut("/api/post/{id}/downvote", (DataService service, int id) =>
{
    return service.PostDownvote(id);
});

// Laver en upvote på et specifik comment på et specifikt comment
app.MapPut("/api/comment/{id}/upvote", (DataService service, int id) =>
{
    return service.CommentUpvote(id);
});

// Laver en downvote på en specifik comment på et specifikt post
app.MapPut("/api/comment/{id}/downvote", (DataService service, int id) =>
{
    return service.CommentDownvote(id);
});


// Henter alle brugerne 
app.MapGet("/api/users", (DataService service) =>
{
    return service.GetUsers();
});

// Henter en specifik kommentar på dets id
app.MapGet("/api/comment/{id}", (DataService service, int id) =>
{
    return service.GetComment(id);
});

// Henter et post på dets id
app.MapGet("/api/post/{id}", (DataService service, int id) =>
{
    return service.GetPost(id);
});

// Starter webapplikationen
app.Run();

// Definition af record-typer

record PostData(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime);
record CommentData(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime commentTime);
record VoteData(int id);
