using ETicaretAPI.API.Configuration.ColumnWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filter;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cors politikas�na ait ayarlar�n yap�lmas�
builder.Services.AddCors(options =>options.AddDefaultPolicy( policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
}));

// Seri log i�in yap�lan ayarlamalar
Logger log = new LoggerConfiguration()
    .WriteTo.Console() // loglar� console yaz
    .WriteTo.File("log/log.txt") // loglar� belirtilen yoldaki dosya i�erisine yaz
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "Logs", needAutoCreateTable: true,
        columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            // Seri log k�t�phanesinin vt de bilgileri kay�t edece�i tablo adlar�
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true,NpgsqlDbType.Varchar)},
            {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            {"user_name", new UsernameColumnWriter()}
        })
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"]) // loglar� g�rselle�tirmek i�in Seq e yazma i�lemi yap�lmaktad�r.
    .Enrich.FromLogContext() // context �zerinden verilen bilgierden yaralanmas� i�in eklenmi�tir.
    .MinimumLevel.Information() // serilog k�t�phanesinin o anki loglam�� oldu�u bilgilerin hangi seviyede oldu�unu belirten ayar
    .CreateLogger();

builder.Host.UseSerilog(log);

// Kullan�c�ya ait t�m bilgileri almak i�in yap�lan gerekli ayarlar.
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});


// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddSignalRServices();

//builder.Services.AddStorage<LocalStorage>();
builder.Services.AddStorage<AzureStorage>();
//builder.Services.AddStorage<ETicaretAPI.Infrastructure.Enums.StorageType.Local>();


// Jwt i�lemleri
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, // olu�turulacak token de�erini kimlerin/hangi originlerin/sitelerin kullanaca��n� belirledi�imiz de�erdir. -> www.bilmem.com 
            ValidateIssuer = true, // olu�turulacak token de�erini kimin da��tt���n� ifade edece�imiz aland�r. -> www.myapi.com(�uanki api �rnek olarak verdi�imiz isim)
            ValidateLifetime = true, // olu�turulacak token de�erinin s�resini kontrol edecek olan de�erdir.
            ValidateIssuerSigningKey = true, // �retilecek token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden security key verisinin do�rulanmas�d�r.


            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = ( notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,
            NameClaimType = ClaimTypes.Name // JWT �zerinde Name Claim ine kar��l�k gelen degeri User.Identity.Name propertysinden elde edebiliriz. Seri log ayarlar� i�in eklendi.
        };
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // wwwroot klas�r�n� kullanmak i�in eklenen middleware

// Custom olu�turulan global exception middleware 
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseSerilogRequestLogging(); // Seri log k�t�phanesinin middleware d�r.Kendinden sonra gelen t�m i�lemlere ait olu�acak bilgileri loglar.

app.UseHttpLogging(); // Kullan�c�ya ait t�m bilgileri almam�za yarayan middleware d�r.

app.UseCors(); // Cors politikas�na ait middleware

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// context �zerinden o anki uygulamay� kullanan kullan�c�n�n ad�n� seri log ayarlar� i�erisine ekledi�imiz vt tablosuna eklememk i�in yaz�lan UsernameColumnWriter class �na iletmemizi sa�lar.
app.Use( async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();
app.MapHubs();

app.Run();
