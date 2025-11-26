using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OrchardCore.DisplayManagement;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrchardCore().AddTheming().AddLiquidViews().AddMvc();

var app = builder.Build();

app.MapGet("/", async (HttpContext context) =>
{
    var factory = context.RequestServices.GetRequiredService<IShapeFactory>();
    var shape = await factory.CreateAsync<Car>("Car", c => { c.Brand = "Renault"; c.Color = "Red";});

    shape.Id = "my-renault";
    shape.TagName = "h3";
    shape.Classes.Add("car");
    shape.Classes.Add("brand-renault");
    shape.Attributes.Add("data-brand", "renault");

    var displayHelper = context.RequestServices.GetRequiredService<IDisplayHelper>();
    var htmlContent = await displayHelper.ShapeExecuteAsync(shape);

    context.Response.ContentType = "text/html";
    context.Response.StatusCode = 200;

    await using var sw = new StreamWriter(context.Response.Body);
    htmlContent.WriteTo(sw, HtmlEncoder.Default);
    
});

app.UseOrchardCore();

app.Run();

public class Car
{
    public string? Brand { get; set; }
    public string? Color { get; set; }
}
