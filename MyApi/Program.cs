using Microsoft.EntityFrameworkCore;
using MyApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Wait for SQL Server and apply pending database migrations.
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext dbContext =
        scope.ServiceProvider.GetRequiredService<AppDbContext>();

    int attempts = 0;
    bool databaseReady = false;

    while (attempts < 10 && !databaseReady)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            databaseReady = true;
        }
        catch (Microsoft.Data.SqlClient.SqlException) when (attempts < 9)
        {
            attempts++;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
