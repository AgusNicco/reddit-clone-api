using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adding CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin() // For development, allow requests from any origin
               .AllowAnyMethod() // Allow all HTTP methods
               .AllowAnyHeader(); // Allow all headers
    });
});

// Other configurations...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS with the specified policy
app.UseCors("AllowAll");

// Other app configurations...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
