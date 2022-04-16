using ETicaretAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Cors politikas�na ait ayarlar�n yap�lmas�
builder.Services.AddCors(options =>options.AddDefaultPolicy( policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
}));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // Cors politikas�na ait middleware

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
