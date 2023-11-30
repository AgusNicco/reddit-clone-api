using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string json = File.ReadAllText("reddit_posts.json");
List<RedditPost> posts = JsonConvert.DeserializeObject<List<RedditPost>>(json);


app.MapGet("/posts/{index}", (int index) =>
{
    index *= 20;
    int totalPosts = posts.Count - 1;
    List<RedditPost> postsToReturn = new List<RedditPost>();
    for (int i = 0; i < 20; i++)
    {
        postsToReturn.Add(posts[index + i % totalPosts]);
    }
    return Results.Ok(postsToReturn);
});

app.MapGet("/posts", () =>
{
    int totalPosts = posts.Count - 1;
    int index = Random.Shared.Next(0, totalPosts);
    List<RedditPost> postsToReturn = new List<RedditPost>();
    for (int i = 0; i < 20; i++)
    {
        postsToReturn.Add(posts[index + i % totalPosts]);
    }
    return Results.Ok(postsToReturn);
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
