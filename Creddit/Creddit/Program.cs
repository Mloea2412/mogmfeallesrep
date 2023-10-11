using Creddit.Model;
using Creddit.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


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

var app = builder.Build();

// Tilføj DbContext factory som service.
builder.Services.AddDbContext<CredditContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Viser flotte fejlbeskeder i browseren hvis der kommer fejl fra databasen
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

// Dette kode kan bruges til at fjerne "cykler" i JSON objekterne.
/*
builder.Services.Configure<JsonOptions>(options =>
{
    // Her kan man fjerne fejl der opstår, når man returnerer JSON med objekter,
    // der refererer til hinanden i en cykel.
    // (altså dobbelrettede associeringer)
    options.SerializerOptions.ReferenceHandler = 
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
*/


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Seed data hvis nødvendigt.
using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData(); // Fylder data på, hvis databasen er tom. Ellers ikke.
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(AllowSomeStuff);

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Middlware der kører før hver request. Sætter ContentType for alle responses til "JSON".
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// DataService fåes via "Dependency Injection" (DI)
app.MapGet("/", (DataService service) =>
{
    return new { message = "Hello World!" };

});

// POSTS
app.MapGet("/api/posts", (DataService service) =>
{
    return service.GetPosts();
});

// POST PÅ ID
app.MapGet("/api/post/{id}", (DataService service, int postid) =>
{
    return service.GetPost(postid);
});

// COMMENTS

app.MapGet("/api/comments", (DataService service) =>
{
    return service.GetComments();
});

app.MapGet("/api/commment/{id}", (DataService service, int commentid) =>
{
    return service.GetComment(commentid);
});

app.MapGet("/api/users", (DataService service) =>
{
    return service.GetUsers();
});

app.MapGet("/api/user/{id}", (DataService service, int userid) =>
{
    return service.GetUsers(userid);
});

/*
app.MapPost("/api/....", (DataService service, ......) =>
{

})
*/

app.Run();

