using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Models;

namespace StudentManagementWeb.Pages.Students;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public List<Student> Students { get; set; } = new();

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        var client = _httpClientFactory.CreateClient("StudentApi");
        var response = await client.GetAsync("/students");

        if (!response.IsSuccessStatusCode) return;

        var json = await response.Content.ReadAsStringAsync();
        Students = JsonSerializer.Deserialize<List<Student>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {
        var client = _httpClientFactory.CreateClient("StudentApi");
        await client.DeleteAsync($"/students/{id}");
        return RedirectToPage();
    }
}