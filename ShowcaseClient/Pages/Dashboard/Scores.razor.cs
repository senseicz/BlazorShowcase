using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace ShowcaseClient.Pages.Dashboard;

public partial class Scores
{

    private RenderFragment Info(string title, string value, bool bordered = false)
    {

        return new RenderFragment(b =>
        {
            b.OpenElement(0, "div");
            b.AddAttribute(1, "class", "headerInfo");

            b.OpenElement(2, "span");
            b.AddContent(3, title);
            b.CloseElement(); //span

            b.OpenElement(4, "p");
            b.AddContent(5, value);
            b.CloseElement(); //p
            
            if (bordered)
            {
                b.OpenElement(6, "em");
                b.CloseElement();
            }
            
            b.CloseElement(); //div
        });

    }

}