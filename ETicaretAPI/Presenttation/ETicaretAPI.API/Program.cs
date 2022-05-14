using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure.Filter;
using ETicaretAPI.Persistence;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Cors politikasýna ait ayarlarýn yapýlmasý
builder.Services.AddCors(options =>options.AddDefaultPolicy( policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
}));

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // Cors politikasýna ait middleware

app.UseStaticFiles(); // wwwroot klasörünü kullanmak için eklenen middleware

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
