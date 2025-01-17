using APICRM.Logic;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using APICRM.Models;
using APICRM.Models.Mapper;


var builder = WebApplication.CreateBuilder(args);

//Add soporte para CORS
builder.Services.AddCors(p => p.AddPolicy("politicaCors", build =>
{
    build.WithOrigins("http://localhost:5046", "https://localhost:7237", "http://172.16.101.20:81", "http://localhost:81")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials(); // No uses AllowAnyOrigin()
}));

//Agregamos el auto mapper
builder.Services.AddAutoMapper(typeof(Mapper));

//Add soporte para cache
builder.Services.AddResponseCaching();

//Add soporte para que se muestre como quiero clases
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Opciones de configuración para Newtonsoft.Json (si las necesitas)
        //options.SerializerSettings.ContractResolver = new DefaultContractResolver();  // Esto es opcional, pero te asegura que las propiedades sean camelCase por defecto.

        options.SerializerSettings.ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new DefaultNamingStrategy() // Aquí se indica que no haya transformación en el nombre de las propiedades
        };

    });


//Add soporte a mi clase logic
builder.Services.AddScoped<Methods>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<RequestExample>(); // Registra los ejemplos en tu proyecto

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Integracion Fresh",
        Version = "v1"
    });

    // Habilitar soporte para anotaciones (como [SwaggerOperation])
    c.EnableAnnotations();

    // Esto te permite agregar ejemplos para tus modelos
    c.ExampleFilters(); // Esto es para añadir los ejemplos personalizados

});

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
