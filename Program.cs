using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseCors( x=> x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());


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
    if (index < 0) 
        index = 0;
    index *= 20;
    int maxIndex = (posts.Count - 1) / 20;
    List<RedditPost> postsToReturn = new List<RedditPost>();
    
    for (int i = 0; i < 20; i++)
    {
        postsToReturn.Add(posts[(index + i) % maxIndex]);
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

app.MapGet("/search/{searchTerm}", (string searchTerm) =>
{
    List<RedditPost> postsToReturn = new List<RedditPost>();
    foreach (RedditPost post in posts)
    {
        if (post.Summary.Contains(searchTerm))
        {
            postsToReturn.Add(post);
        }
        if (postsToReturn.Count >= 20)
            break;
    }
    return Results.Ok(postsToReturn);
});

app.MapGet("/post/{id}", (string id ) =>
{
    RedditPost post = posts.Find(x=> x.Id == id);
    return Results.Ok(post);
});




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
