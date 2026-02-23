using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Courses;

public class CreateModel : PageModel
{
    private readonly ApiClient _api;
    public CreateModel(ApiClient api) => _api = api;

    [BindProperty]
    public CourseCreateDto Course { get; set; } = new("");

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Course.Title))
        {
            ModelState.AddModelError(string.Empty, "Title is required.");
            return Page();
        }

        await _api.CreateCourseAsync(new CourseCreateDto(Course.Title.Trim()));
        return RedirectToPage("Index");
    }
}