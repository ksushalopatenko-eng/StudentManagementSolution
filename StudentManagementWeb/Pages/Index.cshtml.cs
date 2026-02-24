using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Models;

namespace StudentManagementWeb.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Stats? Stats { get; set; }
    public string? Error { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("StudentApi");
            var response = await client.GetAsync("/stats");

            if (!response.IsSuccessStatusCode)
            {
                Error = "Could not load stats";
                return;
            }

            var json = await response.Content.ReadAsStringAsync();

            Stats = JsonSerializer.Deserialize<Stats>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}