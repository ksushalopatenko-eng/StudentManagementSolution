using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentManagementWeb.Pages.Students;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public int Age { get; set; }

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(Name))
            ModelState.AddModelError(nameof(Name), "Name is required.");

        if (Age <= 0)
            ModelState.AddModelError(nameof(Age), "Age must be positive.");

        if (!ModelState.IsValid)
            return Page();

        var client = _httpClientFactory.CreateClient("StudentApi");
        var payload = new { name = Name.Trim(), age = Age };

        var response = await client.PostAsJsonAsync("/students", payload);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "API error while creating student.");
            return Page();
        }

        return RedirectToPage("/Students/Index");
    }
}