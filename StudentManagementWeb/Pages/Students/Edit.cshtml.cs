using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Models;

namespace StudentManagementWeb.Pages.Students;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty] public int Id { get; set; }
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public int Age { get; set; }

    public string Error { get; set; } = "";

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        var client = _httpClientFactory.CreateClient("StudentApi");
        var response = await client.GetAsync($"/students/{id}");

        if (!response.IsSuccessStatusCode)
        {
            Error = "Student not found.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        var student = JsonSerializer.Deserialize<Student>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (student == null)
        {
            Error = "Student not found.";
            return Page();
        }

        Id = student.Id;
        Name = student.Name;
        Age = student.Age;

        return Page();
    }

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

        var response = await client.PutAsJsonAsync($"/students/{Id}", payload);

        if (!response.IsSuccessStatusCode)
        {
            Error = "API error while updating student.";
            return Page();
        }

        return RedirectToPage("/Students/Index");
    }
} 