using Microsoft.EntityFrameworkCore;
using MmReddit.Model;
using MmReddit.Service;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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


app.MapGet("/", (DataService service) =>
{
    return new { message = "Hello World!" };
});

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});



app.MapGet("api/posts", (DataService service) =>
{
    return service.Posts();
});








app.Run();

record PostDTO(string title, User user, string content, int upvote, int downvote, int numberOfVotes, DateTime postTime);
record CommentDTO(string content, int upvote, int downvote, int numberOfVotes, int postid, User user, DateTime commentTime);
record PostVoteDTO(int postid, User user, bool UpvoteOrDownvote);
record CommentVoteDTO(int commentId, User user, bool UpvoteOrDownvote);