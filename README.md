# PIE CHART MAUI
A component pie chart with animation and more.

The library consists of one NuGet packages:

[![NuGet](https://img.shields.io/nuget/v/PieMaui.svg?label=PieMaui)](https://www.nuget.org/packages/PieMaui/)

[Sample](https://github.com/pabloprogramador/Pie/tree/main/Pie.Sample)

To use, simply install the package and add use to your builder:
```javascript
var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      builder.UsePie();
```

<img src="https://github.com/pabloprogramador/pie/blob/main/images/pie.gif" height="600">

You can use the OpenView:
```html
xmlns:pie="clr-namespace:Pie;assembly=Pie"

    <pie:Pie x:Name="pgPie"
        IsHalfCircle="{Binding IsHalfCircle}"
        Values="{Binding Values}"
        PieColor="{Binding PieColor}"
        PieColors="{Binding PieColors}"
        HorizontalOptions="FillAndExpand"
        HeightRequest="400"
    />
```
or more advanced

```html
<pie:Pie x:Name="pgPie"
                IsHalfCircle="{Binding IsHalfCircle}"
                Values="{Binding Values}"
                PieColor="{Binding PieColor}"
                PieColors="{Binding PieColors}"
                HorizontalOptions="FillAndExpand"
                HeightRequest="400"
                TimeAnimation="500"
                SizeCircle="360"
                StrokeWidth="80"
                Round="7"
                Spacing="3"
                MinOpacity=".2"
                MarginWholeCircle="20"
                />
```
