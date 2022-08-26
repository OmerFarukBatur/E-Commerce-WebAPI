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

// Cors politikasýna ait ayarlarýn yapýlmasý
builder.Services.AddCors(options =>options.AddDefaultPolicy( policy =>
{
    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
}));

// Seri log için yapýlan ayarlamalar
Logger log = new LoggerConfiguration()
    .WriteTo.Console() // loglarý console yaz
    .WriteTo.File("log/log.txt") // loglarý belirtilen yoldaki dosya içerisine yaz
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "Logs", needAutoCreateTable: true,
        columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            // Seri log kütüphanesinin vt de bilgileri kayýt edeceði tablo adlarý
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true,NpgsqlDbType.Varchar)},
            {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            {"user_name", new UsernameColumnWriter()}
        })
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"]) // loglarý görselleþtirmek için Seq e yazma iþlemi yapýlmaktadýr.
    .Enrich.FromLogContext() // context üzerinden verilen bilgierden yaralanmasý için eklenmiþtir.
    .MinimumLevel.Information() // serilog kütüphanesinin o anki loglamýþ olduðu bilgilerin hangi seviyede olduðunu belirten ayar
    .CreateLogger();

builder.Host.UseSerilog(log);

// Kullanýcýya ait tüm bilgileri almak için yapýlan gerekli ayarlar.
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


// Jwt iþlemleri
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, // oluþturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanacaðýný belirlediðimiz deðerdir. -> www.bilmem.com 
            ValidateIssuer = true, // oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr. -> www.myapi.com(þuanki api örnek olarak verdiðimiz isim)
            ValidateLifetime = true, // oluþturulacak token deðerinin süresini kontrol edecek olan deðerdir.
            ValidateIssuerSigningKey = true, // üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulanmasýdýr.


            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = ( notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,
            NameClaimType = ClaimTypes.Name // JWT üzerinde Name Claim ine karþýlýk gelen degeri User.Identity.Name propertysinden elde edebiliriz. Seri log ayarlarý için eklendi.
        };
    });

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // wwwroot klasörünü kullanmak için eklenen middleware

// Custom oluþturulan global exception middleware 
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseSerilogRequestLogging(); // Seri log kütüphanesinin middleware dýr.Kendinden sonra gelen tüm iþlemlere ait oluþacak bilgileri loglar.

app.UseHttpLogging(); // Kullanýcýya ait tüm bilgileri almamýza yarayan middleware dýr.

app.UseCors(); // Cors politikasýna ait middleware

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// context üzerinden o anki uygulamayý kullanan kullanýcýnýn adýný seri log ayarlarý içerisine eklediðimiz vt tablosuna eklememk için yazýlan UsernameColumnWriter class ýna iletmemizi saðlar.
app.Use( async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();
app.MapHubs();

app.Run();
