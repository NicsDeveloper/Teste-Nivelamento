using Questao5.Infrastructure.Sqlite;
using Questao5.Application.Interfaces;
using Questao5.Infrastructure.Data;
using System.Reflection;
using Questao5.Application.Movement;

var builder = WebApplication.CreateBuilder(args);

// Configura explicitamente a URL para escutar na porta 5000
builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddControllers();
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssemblyContaining<CreateMovementHandler>());

builder.Services.AddSingleton(new DatabaseConfig
{
    Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite")
});
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IMovementRepository, MovementRepository>();
builder.Services.AddScoped<ICurrentAccountRepository, CurrentAccountRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Questao5 API",
        Version = "v1",
        Description = "API para movimentação e consulta de Balance de conta corrente",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Questao5 API v1");
        options.RoutePrefix = string.Empty;
    });

    var url = "http://localhost:5000";
    await Task.Run(() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
        FileName = url,
        UseShellExecute = true
    }));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAll");

app.Services.GetService<IDatabaseBootstrap>()?.Setup();

app.Run();
