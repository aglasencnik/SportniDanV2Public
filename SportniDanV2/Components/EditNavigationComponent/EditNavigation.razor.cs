using Microsoft.AspNetCore.Components;
using SportniDanV2.Enums;

namespace SportniDanV2.Components.EditNavigationComponent;

public class EditNavigationBase : ComponentBase
{
    [Parameter] public EditPageType CurrentPage { get; set; }
}
