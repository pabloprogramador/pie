using System;
using SkiaSharp.Views.Maui.Controls.Hosting;
namespace Pie;
public static class AppBuilderExtensions
{

    public static MauiAppBuilder UsePie(this MauiAppBuilder builder)
    {

        builder.UseSkiaSharp();
        return builder;
    }
}