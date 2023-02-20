using Microsoft.JSInterop;
using SportniDanV2.Models;

namespace SportniDanV2.Services;

public class BrowserDimensionService
{
    private readonly IJSRuntime _js;

    public BrowserDimensionService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<BrowserDimensionModel> GetDimensions()
    {
        return await _js.InvokeAsync<BrowserDimensionModel>("getDimensions");
    }
}
