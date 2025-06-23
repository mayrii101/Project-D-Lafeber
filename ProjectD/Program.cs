//packages:
//dotnet add package Microsoft.EntityFrameworkCore.SqlServer
//dotnet add package Microsoft.EntityFrameworkCore.SqlServer
//dotnet add package Microsoft.EntityFrameworkCore.Tools
///dotnet add package Microsoft.EntityFrameworkCore.Design

using Microsoft.EntityFrameworkCore;
using ProjectD.Models;
using ProjectD.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");

//CORS voor connectie met frontend 
//AllowAll later aanpassen naar frontend locatie
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//JSON enum converter (enums worden als string gebruikt/gelezen) + Prevent Circular References
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

//Database
/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;" +
                         "Database=LF-Database;" +
                         "User Id= sa;" +
                         "Password=Admin123!;" +
                         "TrustServerCertificate=True;" +
                         "Encrypt=False;" +
                         "Connection Timeout=30;"));
*/
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost\\SQLEXPRESS;" +
                         "Database=LF-Database;" +
                         "Trusted_Connection=True;" +
                         "TrustServerCertificate=True;" +
                         "Encrypt=False;" +
                         "Connection Timeout=30;"));


builder.Services.AddScoped<XmlToSqlImporter>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
builder.Services.AddScoped<IStickyNoteService, StickyNoteService>();


var app = builder.Build();


app.UseRouting();
app.UseCors("AllowAll");
// app.UseAuthorization(); 

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        context.Database.OpenConnection();
        Console.WriteLine("Connection successful!");
        //UNCOMMENT om seeddata te runnen voor lege DB
        //SeedData.Initialize(services, context); 

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


app.Run();

