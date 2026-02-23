using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Courses;

public class DeleteModel : PageModel
{
    private readonly ApiClient _api;
    public DeleteModel(ApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public CourseDto? Course { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Course = await _api.GetCourseAsync(Id);
        if (Course is null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _api.DeleteCourseAsync(Id);
        return RedirectToPage("Index");
    }
}