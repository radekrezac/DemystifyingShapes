# Orchard Core Shapes

Orchard Core doesn't render HTML directly, but instead will usually render something called a **Shape**, which is an object that represents the thing to render and has all the necessary data and metadata to render HTML.
When rendering a Shape, Orchard Core will look for specific **templates**, passing the Shape to this template.
Orchard Core can match with many templates for the same Shape. These potential templates are called **Alternates**.

## What is a Shape
- An object implementing the **IShape** interface
- A **dynamic** data model that contains:
	- **Data** that will be ***rendered*** by ASP.NET views
	- **Metadata** on ***how*** to render it

## Benefits of Shapes
- No view name is hard-codded - view name is based on a shape
- Priority based view resolution - Alternates
- Theming - User defined Templates (views)
- Dynamic caching
- Wrapping
- Placement
	- Zones
	- Ordering
- Multiple sources
	- Database
	- Files
	- Code
- Events

## Creating and rendering shapes
1. Create a shape by shape factory and name it
	- `	var factory = context.RequestServices.GetREquiredService<IDisplayHelper>();`
	- ` var shape = await factory.CreateAsync("Car");`
2. Create a HTML content by display helper
	- ` var displayHelper = context.RequestServices.GetRequiredService<IDisplayHelper>();`
	- ` var htmlContent = await displayHelper.ShapeExecuteAsync(shape);`
5. Create a view according to shape name : `Car.cshtml`
6. Send a HTML conent to Response Body
	- ` await using var sw = new StreamWriter(context.Response.Body);`
	- ` htmlContent.WriteTo(sw, HtmlEncoder.Default);` 

Code is available on Github in the branch: [Test_Shape_With_Razor_View](https://github.com/radekrezac/DemystifyingShapes/tree/Test_Shape_With_Razor_View)

## Rendering shapes with Liquid templates
Liquid is a safe, customer-facing templating language originally created by [Shopify](https://shopify.github.io/liquid/). It's designed to be **secure**, **flexible**, and easy to understand, making it **perfect for generating dynamic content** where you need to combine static templates with variable data from your application. 
To render shapes using a liquid templates we only add to dependencies `OrchardCore.DisplayManagement.Liquid` package and to Program.cs `AddLiquidViews()` service.

## Add data to shapes
The shape as an instance of the **IShape** interface contains the `Properties` property of type `IDictionary`, where we can insert data via the index, which is then visible in the template. In the template, then we display data using the 'Model' object. The `ShapeExecuteAsync` method sends a `Shape` model to the template, which, in addition to `IShape`, is also an instance of the `DynamicObject` class of `Compose` containing the `TryGetMember` method. This allows us to access Properties directly, e.g., `Model.Brand` instead of `Model.Properties["Brand"]`.

Code for this part is available on Github in the branch: [Test_ShapeData_With_Liquid_Template](https://github.com/radekrezac/DemystifyingShapes/tree/Test_ShapeData_With_Liquid_Template)

## Strongly typed shapes 
Since the dynamic approach requires some overhead, we will use a generic method `CreateAsync` to create the shape and use a POCO object as the type. In our case, the `Car` class. 


```csharp
public class Car
{
    public string? Brand { get; set; }
    public string? Color { get; set; }
}
var shape = await factory.CreateAsync<Car>("Car", c => { c.Brand = "Renault"; c.Color = "Red";});
```

Now we can type the model as Car and get the value directly from the class properties:

```csharp
@using OrchardCore.DisplayManagement;
@model Car

This is a car @Model.Brand with @Model.Color color
```

## Adding metadata to a shape
As mentioned 'IShape' contains metadata, like `Id`, `TagName`, `Classes`, `Attributes`... , that can be used to render a shape by template. In a template we then render these metadata with helper class. Here is code setting metadata

```csharp
   shape.Id = "my-renault";
   shape.TagName = "h3";
   shape.Classes.Add("car");
   shape.Classes.Add("brand-renault");
   shape.Attributes.Add("data-brand", "renault");
```
and here templates rendering them:


```csharp
@using OrchardCore.DisplayManagement;
@model Car

@{
    var shape = Model as IShape;
    var tagBuilder = shape.GetTagBuilder();
}

@tagBuilder.RenderStartTag()
This is a car @Model.Brand with @Model.Color
@tagBuilder.RenderEndTag()
```
The result is this content:
![Rendered Car Shape](https://github.com/radekrezac/DemystifyingShapes/tree/master/images/rendered-car.jpg?raw=true)

and if we look at the HTML code, we can see the rendered shape metadata:
 ![Rendered Car Shape Source View](https://github.com/radekrezac/DemystifyingShapes/tree/master/images/rendered-car-source.jpg?raw=true)

Code for this part is available on Github in the branch: [Test_Shape_With_Metadata](https://github.com/radekrezac/DemystifyingShapes/tree/Test_Shape_With_Metadata)

References:
------------
- [Orchard Harvest 2024: Demystifying Shapes, Part 1](https://www.youtube.com/watch?v=yaZhKuD2qoI&list=PLpCsCyd254FpDNAMH_Pat0YADI2jMWTTT&index=6)
