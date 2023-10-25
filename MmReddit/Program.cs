using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MmReddit.Model;
using MmReddit.Service;
using Microsoft.AspNetCore.Components.Forms;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Tilføj DbContext factory som service.
builder.Services.AddDbContext<MmRedditContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData();
}

using (var db = new MmRedditContext())
{

}

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

app.MapGet("/api/posts", (DataService service) =>
{
    return service.GetPosts();
});


// HENTER EN BRUGER PÅ DETS ID 
app.MapGet("/api/user/{id}", (DataService services, int id) =>
{
    return services.GetUserId(id);
});

// OPRETTER ET NYT POST
app.MapPost("/api/post/", (DataService service, PostData data) =>
{
    return service.CreatePost(data.title, data.user, data.content, data.upvotes, data.downvotes, data.numberOfVotes, data.postTime);
});

// OPRETTER EN NY KOMMENTAR PÅ ET SPECCIFIKT POST
app.MapPost("/api/post/{id}/comment", (DataService service, CommentData data) =>
{
    return service.CreateComment(data.content, data.upvotes, data.downvotes, data.numberOfVotes, data.postid, data.user, data.commentTime);
});

// LAVER EN UPVOTE PÅ ET SPECIFIKT POST
app.MapPut("/api/post/{id}/upvote", (DataService service, int id) =>
{
    return service.PostUpvote(id);
});

// LAVER ET DOWNVOTE PÅ ET SPECIFIKT POST
app.MapPut("/api/post/{id}/downvote", (DataService service, int id) =>
{
    return service.PostDownvote(id);
});

// LAVER EN UPVOTE PÅ EN SPECIKIK COMMENT PÅ ET SPECIFIKT COMMENT
app.MapPut("/api/comment/{id}/upvote", (DataService service, int id) =>
{
    return service.CommentUpvote(id);
});

// LAVER EN DOWNVOTE PÅ EN SPECIFIK COMMENT PÅ ET SPECIFIKT POST
app.MapPut("/api/comment/{id}/downvote", (DataService service, int id) =>
{
    return service.CommentDownvote(id);
});


// EKSTRA

// HENTER ALLE BRUGERNE
app.MapGet("/api/users", (DataService service) =>
{
    return service.GetUsers();
});

// HENTER KOMMENTAR PÅ DETS ID
app.MapGet("/api/comment/{id}", (DataService service, int id) =>
{
    return service.GetComment(id);
});

// HENTER ET POST PÅ DETS ID
app.MapGet("/api/post/{id}", (DataService service, int id) =>
{
    return service.GetPost(id);
});


// Start webapplikationen
app.Run();


record PostData(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime);
record CommentData(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime commentTime);

record VoteData(int id);