using StarFederation.Datastar.DependencyInjection;
using StarFederation.Datastar.ModelBinding;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDatastar();
builder.Services.AddDatastarMvc();
WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
