using APICRM.Logic;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Add soporte para CORS
builder.Services.AddCors(p => p.AddPolicy("politicaCors", build =>
{
    build.WithOrigins("http://localhost:5046", "https://localhost:7237", "http://172.16.101.20:81", "http://localhost:81")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials(); // No uses AllowAnyOrigin()
}));

//Add soporte para cache
builder.Services.AddResponseCaching();

//Add soporte para que se muestre como quiero clases
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Opciones de configuración para Newtonsoft.Json (si las necesitas)
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();  // Esto es opcional, pero te asegura que las propiedades sean camelCase por defecto.
    });

//Add soporte a mi clase logic
builder.Services.AddScoped<Methods>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api V1");
});

//soporte para CORS
app.UseCors("politicaCors");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
