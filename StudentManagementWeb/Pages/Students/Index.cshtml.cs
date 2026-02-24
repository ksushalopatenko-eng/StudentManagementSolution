using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Models;

namespace StudentManagementWeb.Pages.Students;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // Always shown table
    public List<Student> AllStudents { get; set; } = new();

    // Shown only when Search is not empty
    public List<Student> SearchResults { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public bool HasSearch => !string.IsNullOrWhiteSpace(Search);
    public bool HasResults => SearchResults.Count > 0;

    public async Task OnGet()
    {
        var client = _httpClientFactory.CreateClient("StudentApi");
        var response = await client.GetAsync("/students");

        if (!response.IsSuccessStatusCode) return;

        var json = await response.Content.ReadAsStringAsync();
        var all = JsonSerializer.Deserialize<List<Student>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        AllStudents = all;

        if (HasSearch)
        {
            var term = Search!.Trim();
            SearchResults = all
                .Where(s => s.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {
        var client = _httpClientFactory.CreateClient("StudentApi");
        await client.DeleteAsync($"/students/{id}");

        // Keep search query after delete (nice UX)
        return RedirectToPage(new { Search });
    }
}