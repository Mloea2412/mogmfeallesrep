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


app.MapGet("/api/users", (DataService service) =>
{
    return service.GetUsers();
});

app.MapGet("/api/comment/{id}", (DataService service, int id) =>
{
    return service.GetComment(id);
});   
app.MapGet("/api/post/{id}", (DataService service, int id) =>
{
    return service.GetPost(id);
});

app.MapGet("/api/user/{id}", (DataService services, int id) =>
{
    return services.GetUserId(id);
});


app.MapPost("/api/post/", (DataService service, PostDTO data) =>
{
    return service.CreatePost(data.title, data.user, data.content, data.upvotes, data.downvotes, data.numberOfVotes, data.postTime);
});


app.MapPost("/api/comment", (DataService service, CommentDTO data) =>
{
    return service.CreateComment(data.content, data.upvotes, data.downvotes, data.numberOfVotes, data.postid, data.user, data.commentTime);
});

app.MapPut("/api/post/{id}/upvote", (DataService service) =>
{
    return;
});

app.MapPut("/api/comment/{id}/upvote", (DataService service) =>
{
    return;
});

// Start webapplikationen
app.Run();


record PostDTO(string title, User user, string content, int upvotes, int downvotes, int numberOfVotes, DateTime postTime);
record CommentDTO(string content, int upvotes, int downvotes, int numberOfVotes, int postid, User user, DateTime commentTime);