using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OrchardCore.DisplayManagement;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrchardCore().AddTheming().AddLiquidViews().AddMvc();

var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    var factory = context.RequestServices.GetRequiredService<IShapeFactory>();
    var carShape = await factory.CreateAsync("Car");
    carShape.Properties["Brand"] = "Renault";


    var displayHelper = context.RequestServices.GetRequiredService<IDisplayHelper>();
    var htmlContent = await displayHelper.ShapeExecuteAsync(carShape);

    context.Response.ContentType = "text/html";
    context.Response.StatusCode = 200;

    await using var sw = new StreamWriter(context.Response.Body);
    htmlContent.WriteTo(sw, HtmlEncoder.Default);
    
});

app.UseOrchardCore();

app.Run();
