using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Courses;

public class EditModel : PageModel
{
    private readonly ApiClient _api;
    public EditModel(ApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public CourseUpdateDto Course { get; set; } = new("");

    public async Task<IActionResult> OnGetAsync()
    {
        var course = await _api.GetCourseAsync(Id);
        if (course is null) return NotFound();

        Course = new CourseUpdateDto(course.Title);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Course.Title))
        {
            ModelState.AddModelError(string.Empty, "Title is required.");
            return Page();
        }

        await _api.UpdateCourseAsync(Id, new CourseUpdateDto(Course.Title.Trim()));
        return RedirectToPage("Index");
    }
}